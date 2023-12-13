using AutoMapper;
using Entity;
using FinurligaFinanserWebAPI.DtoModels.TransactionDTOs;
using Infrastructure.Enums;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FinurligaFinanserWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<TransactionController> _logger;
        private readonly IMapper _mapper;

        public TransactionController(ITransactionRepository transactionRepository, ILogger<TransactionController> logger, IMapper mapper)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }

        [HttpPost("Deposit")]
        public async Task<ActionResult<DepositConfirmationDTO>> Deposit(DepositDTO depositDTO)
        {
            var transactionObject = new Transaction(depositDTO.ReceivingAccountNumber, null, depositDTO.Amount, null);
            transactionObject.Type = TransactionType.Deposit;
            var response = await _transactionRepository.Deposit(transactionObject);
            var confirmedDeposit = _mapper.Map<DepositConfirmationDTO>(response);
            return Ok(confirmedDeposit);
        }

        [HttpPost]
        public async Task<ActionResult<TransactionConfirmationDTO>> PostTransaction(PostTransactionDTO postTransactionDto)
        {
            try
            {
                var (createdTransaction, transactionStatus) = await _transactionRepository.CreateTransaction(_mapper.Map<Transaction>(postTransactionDto));

                if (transactionStatus != TransactionStatus.Success) _logger.LogError("Error creating transaction: {TransactionStatus}", transactionStatus);

                if (transactionStatus == TransactionStatus.Invalid_Amount) return BadRequest("Error, trying to send an invalid amount.");
                if (transactionStatus == TransactionStatus.BankAccount_Not_Found) return NotFound("Error, BankAccount Not Found.");
                if (transactionStatus == TransactionStatus.Insufficient_Funds) return BadRequest("Error, trying to send an amount that does not exist on the sender's bankaccount. Insufficient Amount.");

                _logger.LogInformation("Transaction created: {Transaction}", createdTransaction);
                var transactionConfirmationDTO = _mapper.Map<TransactionConfirmationDTO>(createdTransaction);

                return CreatedAtAction("GetTransaction", new {id = createdTransaction!.Id}, transactionConfirmationDTO);
            }
            catch (Exception e)
            {
                _logger.LogError("Error creating transaction: {Message}", e.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionConfirmationDTO>> GetTransaction(int id)
        {
            try
            {
                var transaction = await _transactionRepository.GetTransaction(id);
                if (transaction == null)
                {
                    _logger.LogError("Transaction not found: {Id}", id);
                    return NotFound("Transaction not found");
                }

                _logger.LogInformation("Transaction fetched successfully: {Transaction}", transaction);
                return Ok(_mapper.Map<TransactionConfirmationDTO>(transaction));
            }
            catch (Exception e)
            {
                _logger.LogError("Error fetching transaction: {Message}", e.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("BankAccount/{id}")]
        public async Task<ActionResult<IEnumerable<TransactionConfirmationDTO>>> GetTransactionsByBankAccountId(int id)
        {
            try
            {
                var transactions = await _transactionRepository.GetTransactionsByBankAccountId(id);
                if (transactions == null)
                {
                    _logger.LogError("Transactions not found: {Id}", id);
                    return NotFound("Transactions not found");
                }

                _logger.LogInformation("Transactions fetched successfully: {Transactions}", transactions);
                return Ok(_mapper.Map<IEnumerable<TransactionConfirmationDTO>>(transactions));
            }
            catch (Exception e)
            {
                _logger.LogError("Error fetching transactions: {Message}", e.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
