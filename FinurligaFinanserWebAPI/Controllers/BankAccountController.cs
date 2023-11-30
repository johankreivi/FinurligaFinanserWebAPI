using AutoMapper;
using FinurligaFinanserWebAPI.DtoModels.BankAccountDTOs;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Repositories;

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
                var createdBankAccount = await _bankAccountRepository.CreateBankAccount(bankAccountDto.NameOfAccount, bankAccountDto.UserAccountId);

                var getBankAccount = _mapper.Map<BankAccountDTO>(createdBankAccount);

                return CreatedAtAction("GetBankAccount", new { id = getBankAccount.Id }, getBankAccount);

            }
            catch (Exception e)
            {
                _logger.LogError("Error creating bank account: {Message}", e.Message); // Fixa lite här
                if (e.Message.Equals("400BankAccountName")) return BadRequest("Bank account name is invalid.");
                if (e.Message.Equals("400UserAccountId")) return BadRequest("User id is invalid.");
                if (e.Message.Equals("404UserAccount")) return NotFound("User account not found.");
                return StatusCode(500, "Internal Server Error");
            }           

        
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BankAccountDTO>> GetBankAccount(int id)
        {
            try
            {
                var bankAccount = await _bankAccountRepository.GetBankAccount(id);
                
                return Ok(_mapper.Map<BankAccountDTO>(bankAccount));
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
