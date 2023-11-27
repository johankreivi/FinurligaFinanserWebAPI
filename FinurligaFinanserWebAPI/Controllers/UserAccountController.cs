using AutoMapper;
using Entity;
using FinurligaFinanserWebAPI.DtoModels;
using Infrastructure.Helpers;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FinurligaFinanserWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAccountController : ControllerBase
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        public UserAccountController(IUserAccountRepository userAccountRepository, ILogger logger, IMapper mapper)
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
        public async Task<ActionResult<UserAccountConfirmationDTO>> CreateUserAccount(UserAccountDto userAccountDto)
        {
            _logger.LogInformation("Attempting to create a new user account.");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                byte[] passwordSalt = PasswordHasher.GenerateSalt();
                string passwordHash = PasswordHasher.HashPassword(userAccountDto.Password, passwordSalt);

                var userAccount = new UserAccount ( userAccountDto.UserName, userAccountDto.FirstName, userAccountDto.LastName, passwordSalt, passwordHash );

                var createdUserAccount = await _userAccountRepository.CreateUserAccount(userAccount);

                if (createdUserAccount == null)
                {
                    _logger.LogError("User account creation failed. The repository method returned null.");
                    return StatusCode(500, "Error creating the user account.");
                }

                var confirmationDTO = _mapper.Map<UserAccountConfirmationDTO>(createdUserAccount);

                return CreatedAtAction(nameof(GetOneUser), new { id = confirmationDTO.Id }, confirmationDTO);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while attempting to create a new user account.");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
