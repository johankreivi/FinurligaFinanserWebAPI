using Entity;
using Infrastructure.Enums;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions; 

namespace Infrastructure.Repositories
{
    public partial class UserAccountRepository : IUserAccountRepository
    {
        private readonly ILogger _logger;
        private readonly DataContext _dataContext;
        private const int USERNAME_MIN_LENGTH = 6;

        [GeneratedRegex("^[a-zA-ZäöåÄÖÅ]+$")]
        private static partial Regex CheckForInvalidLetters();
        [GeneratedRegex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).+$")]
        private static partial Regex PasswordIsValid();

        public UserAccountRepository(DataContext dataContext, ILogger<UserAccountRepository> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        public async Task<List<UserAccount>> GetAllUserAccountsAsync(int take) 
        {

            return await _dataContext.UserAccounts.Take(take).Include(x => x.BankAccounts).ToListAsync(); 
        }

        public async Task<UserAccount?> GetOneUser(int id) => await _dataContext.UserAccounts.FindAsync(id);

        private UserValidationStatus ValidateUser(UserAccount userAccount, string password)
        {
            var userNameResult = ValidateUserName(userAccount.UserName);
            if (userNameResult != UserValidationStatus.Valid) return userNameResult;

            var firstNameResult = ValidateName(userAccount.FirstName);
            if (firstNameResult != UserValidationStatus.Valid) return firstNameResult;

            var lastNameResult = ValidateName(userAccount.LastName);
            if (lastNameResult != UserValidationStatus.Valid) return lastNameResult;

            var passwordResult = ValidatePassword(password);
            if (passwordResult != UserValidationStatus.Valid) return passwordResult;

            return UserValidationStatus.Valid;
        }

        private UserValidationStatus ValidateUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName)) 
                return UserValidationStatus.NotValid_UserName_Is_NullOrEmpty;

            if (userName.Length < USERNAME_MIN_LENGTH)
                return UserValidationStatus.NotValid_UserNameLength_Too_Short;

            return UserValidationStatus.Valid;
        }

        private UserValidationStatus ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return UserValidationStatus.NotValid_Name_Is_NullOrEmpty;

            if (name.Length < 2)
                return UserValidationStatus.NotValid_NameLength_Too_Short;

            if (!CheckForInvalidLetters().IsMatch(name))
                return UserValidationStatus.NotValid_Name_Contains_Invalid_Characters;
            
            return UserValidationStatus.Valid;
        }

        private UserValidationStatus ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || !PasswordIsValid().IsMatch(password))
                return UserValidationStatus.NotValid_Password_Does_Not_Meet_Requirements;
            
            return UserValidationStatus.Valid;
        }

        public async Task<(UserAccount, UserValidationStatus)> CreateUserAccount(string userName, string firstName, string lastName, string password)
        {
            try
            {
                byte[] passwordSalt = PasswordHasher.GenerateSalt();
                string passwordHash = PasswordHasher.HashPassword(password, passwordSalt);

                var userAccount = new UserAccount(userName, firstName, lastName, passwordSalt, passwordHash);

                var validationStatus = ValidateUser(userAccount, password);

                if(_dataContext.UserAccounts.Any(x => x.UserName == userAccount.UserName))
                {
                    return (userAccount, UserValidationStatus.NotValid_UserName_Already_Taken);
                }

                if (validationStatus == UserValidationStatus.Valid)
                {
                    _dataContext.UserAccounts.Add(userAccount);
                    await _dataContext.SaveChangesAsync();
                }

                return (userAccount, validationStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception was thrown when attempting to create a new UserAccount.");
                throw;
            }
        }

        public async Task<bool> AuthorizeUserLogin(string userName, string password)
        {
            try
            {
                var userInDb = await _dataContext.UserAccounts.FirstOrDefaultAsync(x => x.UserName == userName);

                if(userInDb == null)
                {
                    return false;
                }
                                    
                var hashedPassword = PasswordHasher.HashPassword(password, userInDb.PasswordSalt);

                return hashedPassword == userInDb.PasswordHash;
            }
            catch (Exception ex)
            {                
                throw new Exception("An error occurred while processing your request.", ex);
            }
        }

        public async Task<int> GetUserId(string userName)
        {
            var user = await _dataContext.UserAccounts.FirstOrDefaultAsync(x => x.UserName == userName);
            return user is not null ? user.Id : -1;
        }
    }
}
