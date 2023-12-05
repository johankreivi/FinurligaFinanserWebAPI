using AutoMapper;
using Entity;
using FinurligaFinanserWebAPI.DtoModels.TransactionDTOs;
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
            var transactionObject = new Transaction(depositDTO.ReceivingAccountNumber, null, depositDTO.Amount, TransactionType.Deposit, null);
            var response = await _transactionRepository.Deposit(transactionObject);
            var confirmedDeposit = new DepositConfirmationDTO(response.Type, response.ReceivingAccountNumber, response.Amount, response.TimeStamp);
            return Ok(confirmedDeposit);
        }
    }
}
