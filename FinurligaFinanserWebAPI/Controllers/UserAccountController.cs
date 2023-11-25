using Entity;
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
        public UserAccountController(IUserAccountRepository userAccountRepository, ILogger logger)
        {
            _logger = logger;
            _userAccountRepository = userAccountRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserAccount>>> GetAll(int take = 10)
        {
            var result = await _userAccountRepository.GetAllUserAccountsAsync(take);

            return Ok(result);
        }
    }
}
