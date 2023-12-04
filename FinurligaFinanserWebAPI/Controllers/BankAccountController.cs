using AutoMapper;
using FinurligaFinanserWebAPI.DtoModels.BankAccountDTOs;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Repositories;
using Infrastructure.Enums;

namespace FinurligaFinanserWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountController : ControllerBase
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly ILogger<BankAccountController> _logger;
        private readonly IMapper _mapper;
        public BankAccountController(IBankAccountRepository bankAccountRepository, ILogger<BankAccountController> logger, IMapper mapper)
        {
            _logger = logger;
            _bankAccountRepository = bankAccountRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<BankAccountDTO>> CreateBankAccount(PostBankAccountDTO bankAccountDto)
        {            
            try
            {
                var (createdBankAccount, validationStatus) = await _bankAccountRepository.CreateBankAccount(bankAccountDto.NameOfAccount, bankAccountDto.UserAccountId);

                if (validationStatus != BankAccountValidationStatus.Valid) _logger.LogError("Error creating bank account: {ValidationStatus}", validationStatus);

                if (validationStatus == BankAccountValidationStatus.Invalid_BankAccountName) return BadRequest("Bank account name is invalid.");
                if (validationStatus == BankAccountValidationStatus.Invalid_UserAccountId) return BadRequest("User id is invalid.");
                if (validationStatus == BankAccountValidationStatus.NotFound) return NotFound("User account not found.");

                _logger.LogInformation("Bank account created: {BankAccount}", createdBankAccount);

                var getBankAccount = _mapper.Map<BankAccountDTO>(createdBankAccount);

                return CreatedAtAction("GetBankAccount", new { id = getBankAccount.Id }, getBankAccount);

            }
            catch (Exception e)
            {
                _logger.LogError("Error creating bank account: {Message}", e.Message);
                return StatusCode(500, "Internal Server Error");
            }           

        
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BankAccountDTO>> GetBankAccount(int id)
        {
            try
            {
                var bankAccount = await _bankAccountRepository.GetBankAccount(id);
                if (bankAccount == null)
                {
                    _logger.LogError("Bank account not found: {Id}", id);
                    return NotFound("Bank account not found.");
                }

                _logger.LogInformation("Bank account found: {BankAccount}", bankAccount);

                return Ok(_mapper.Map<BankAccountDTO>(bankAccount));
            }
            catch (Exception e)
            {
                _logger.LogError("Error getting bank account: {Message}", e.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("User/{id}")]
        public async Task<ActionResult<IEnumerable<BankAccountDTO>>> GetAllBankAccounts(int id)
        {
            try
            {
                var bankAccounts = await _bankAccountRepository.GetAllBankAccounts(id);
                if (bankAccounts == null)
                {
                    _logger.LogError("Bank accounts not found: {Id}", id);
                    return NotFound("Bank accounts not found.");
                }

                _logger.LogInformation("Bank accounts found: {BankAccounts}", bankAccounts);

                return Ok(_mapper.Map<IEnumerable<BankAccountDTO>>(bankAccounts));
            }
            catch (Exception e)
            {
                _logger.LogError("Error getting bank accounts: {Message}", e.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
