using Entity;
using Infrastructure.Enums;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Infrastructure.Repositories
{
    public partial class UserAccountRepository : IUserAccountRepository
    {
        private readonly DataContext _dataContext;
        private const int USERNAME_MIN_LENGTH = 6;

        [GeneratedRegex("^[a-zA-ZäöåÄÖÅ]+$")]
        private static partial Regex CheckForInvalidLetters();
        [GeneratedRegex("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).+$")]
        private static partial Regex PasswordIsValid();

        public UserAccountRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<UserAccount>> GetAllUserAccountsAsync(int take = 10) 
        {

            return await _dataContext.UserAccounts.Take(take).ToListAsync(); 
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
    }
}
