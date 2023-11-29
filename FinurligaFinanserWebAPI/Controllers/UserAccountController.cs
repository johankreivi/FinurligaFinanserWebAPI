using AutoMapper;
using Entity;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Enums;
using static Infrastructure.Repositories.UserAccountRepository;
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

        [HttpGet("GetALL")]
        public async Task<ActionResult<List<UserAccount>>> GetAll(int take = 10)
        {
            var result = await _userAccountRepository.GetAllUserAccountsAsync(take);

            return Ok(result);
        }

        [HttpGet("GetOne")]
        public async Task<ActionResult> GetOneUser(int id)
        {
            var result = await _userAccountRepository.GetOneUser(id);

            return Ok(result);
        }

        [HttpPost("CreateUserAccount")]
        public async Task<ActionResult<UserAccountConfirmationDTO>> CreateUserAccount(UserAccountDTO userAccountDto)
        {
            _logger.LogInformation("Attempting to create a new user account.");

            // Kontrollerar att inskickat objekt är i valid state.
            if (!ModelState.IsValid) return BadRequest(ModelState);            

            try
            {
                var accountCreationResponse = await _userAccountRepository.CreateUserAccount(
                    userAccountDto.UserName, userAccountDto.FirstName, userAccountDto.LastName,userAccountDto.Password);

                var userAccount = accountCreationResponse.Item1;
                var validationStatus = accountCreationResponse.Item2;

                if (validationStatus != UserValidationStatus.Valid)
                {
                    _logger.LogError("User account creation failed due to: {ValidationStatus}", validationStatus);
                    return BadRequest($"Error creating the user account. Error: {validationStatus}");
                }
                
                var confirmationDTO = _mapper.Map<UserAccountConfirmationDTO>(userAccount);

                return CreatedAtAction(nameof(GetOneUser), new { id = confirmationDTO.Id }, confirmationDTO);
            }

            catch (UserNameAlreadyExistsException uex)
            {
                return BadRequest(uex.Message);
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
                        
            var isLoginSuccess = await _userAccountRepository.AuthorizeUserLogin(loginUser.UserName, loginUser.Password);

            var result = new LoginUserConfirmationDTO { UserName = loginUser.UserName, IsAuthorized = isLoginSuccess};

            if (isLoginSuccess)
            {
                result.Message = $"User {loginUser.UserName}, access granted!";
                return Ok(result);
            }
                        
            result.Message = "Invalid username or password";
            return Unauthorized(result);
        }        
    }
}
