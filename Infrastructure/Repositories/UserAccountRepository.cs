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

        public async Task<UserAccount> GetOneUser(int id)
        {
            return await _dataContext.UserAccounts.FindAsync(id);
        }

        public async Task<UserValidationStatus> RegisterUser(string userName, string firstName, string lastName, string password)
        {
            var validationResult = (ValidateUser(userName, firstName, lastName, password));
            if (validationResult != UserValidationStatus.Valid)
                return validationResult;

            var passwordSalt = PasswordHasher.GenerateSalt();
            var passwordHash = PasswordHasher.HashPassword(password, passwordSalt);

            UserAccount userAccount = new(userName, firstName, lastName, passwordSalt, passwordHash);
            
            await _dataContext.UserAccounts.AddAsync(userAccount);
            await _dataContext.SaveChangesAsync();

            return UserValidationStatus.Valid;
        }

        private UserValidationStatus ValidateUser(string userName, string firstName, string lastName, string password)
        {
            var userNameResult = ValidateUserName(userName);
            if (userNameResult != UserValidationStatus.Valid) return userNameResult;

            var firstNameResult = ValidateName(firstName);
            if (firstNameResult != UserValidationStatus.Valid) return firstNameResult;

            var lastNameResult = ValidateName(lastName);
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

        public class UserNameAlreadyExistsException : Exception
        {
            public UserNameAlreadyExistsException(string message)
                : base(message)
            {
            }
        }

        public async Task<UserAccount> CreateUserAccount(UserAccount userAccount)
        {
            try
            {
                _dataContext.UserAccounts.Add(userAccount);
                await _dataContext.SaveChangesAsync();                

                return userAccount;
            }
            catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx &&
                                               (sqlEx.Number == 2627 || sqlEx.Number == 2601))            {
                
                _logger.LogError(ex, "Ett undantag inträffade när ett nytt användarkonto skulle skapas på grund av duplicerat användarnamn.");

                throw new UserNameAlreadyExistsException("Användarnamnet är upptaget. Välj ett annat namn.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ett oväntat undantag inträffade när ett nytt användarkonto skulle skapas.");
                throw;
            }
        }
    }
}
