using AutoMapper;
using Entity;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Enums;
using FinurligaFinanserWebAPI.DtoModels.UserAccountDTOs;

namespace FinurligaFinanserWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccountController : ControllerBase
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly ILogger<UserAccountController> _logger;
        private readonly IMapper _mapper;
        public UserAccountController(IUserAccountRepository userAccountRepository, ILogger<UserAccountController> logger, IMapper mapper)
        {
            _logger = logger;
            _userAccountRepository = userAccountRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserAccount>>> GetAll(int take = 10)
        {
            var result = await _userAccountRepository.GetAllUserAccountsAsync(take);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserAccountConfirmationDTO>> GetOneUser(int id)
        {
            try
            {
                var result = await _userAccountRepository.GetOneUser(id);
                if (result == null)
                {
                    _logger.LogError("User account not found: {Id}", id);
                    return NotFound();
                }
                _logger.LogInformation("User account found: {UserAccount}", result);

                var userAccountConfirmationDTO = _mapper.Map<UserAccountConfirmationDTO>(result);
                return Ok(userAccountConfirmationDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while attempting to get one user account.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserAccountConfirmationDTO>> CreateUserAccount(UserAccountDTO userAccountDto)
        {
            _logger.LogInformation("Attempting to create a new user account.");

            if (!ModelState.IsValid) return BadRequest(ModelState);            

            try
            {
                var (userAccount, validationStatus) = await _userAccountRepository.CreateUserAccount(
                                       userAccountDto.UserName, userAccountDto.FirstName, userAccountDto.LastName, userAccountDto.Password);

                if (validationStatus != UserValidationStatus.Valid)
                {
                    _logger.LogError("User account creation failed due to: {ValidationStatus}", validationStatus);
                    return BadRequest($"Error creating the user account. Error: {validationStatus}");
                }
                
                var confirmationDTO = _mapper.Map<UserAccountConfirmationDTO>(userAccount);

                return CreatedAtAction(nameof(GetOneUser), new { id = confirmationDTO.Id }, confirmationDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while attempting to create a new user account.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginUserConfirmationDTO>> Login(LoginUserDTO loginUser)
        {            
            if (string.IsNullOrEmpty(loginUser.UserName) || string.IsNullOrEmpty(loginUser.Password))
            {
                return BadRequest("Username or password cannot be null or empty");
            }

            int userId = await _userAccountRepository.GetUserId(loginUser.UserName);
                        
            var isLoginSuccess = await _userAccountRepository.AuthorizeUserLogin(loginUser.UserName, loginUser.Password);

            var result = new LoginUserConfirmationDTO {Id=userId, UserName = loginUser.UserName, IsAuthorized = isLoginSuccess};

            if (isLoginSuccess)
            {
                result.Message = $"User {loginUser.UserName}, access granted!";
                return Ok(result);
            }
                        
            result.Message = "Invalid username or password";
            return Unauthorized(result);
        }

        [HttpGet("Info/{id}")]
        public async Task<ActionResult<UserAccountDetailsDTO>> GetUserDetails(int id)
        {
            var getInfo = await _userAccountRepository.GetUserDetails(id);
            var details = _mapper.Map<UserAccountDetailsDTO>(getInfo);
            return Ok(details);
        }

        [HttpGet("BankAccount/{bankAccountNumber}")]
        public async Task<ActionResult<UserAccountDetailsDTO>> GetUserAccountByBankAccountNumber(int bankAccountNumber)
        {            
            var userAccountId = await _userAccountRepository.GetUserAccountByBankAccountNumber(bankAccountNumber);
                        
            if (userAccountId == 0) return NotFound("Hittade inget user account som matchade bankkontonumret");
            
            var userDetailsResult = await GetUserDetails(userAccountId);
            
            return userDetailsResult;
        }
    }
}
