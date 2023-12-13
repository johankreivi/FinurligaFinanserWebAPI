using Entity;
using Infrastructure.Enums;

namespace Infrastructure.Repositories
{
    public interface IUserAccountRepository
    {
        Task<List<UserAccount>> GetAllUserAccountsAsync(int take);
        Task<(UserAccount, UserValidationStatus)> CreateUserAccount(string userName, string firstName, string lastName, string password);
        Task<UserAccount?> GetOneUser(int id);
        Task<bool> AuthorizeUserLogin(string userName, string password);
        Task<int> GetUserId(string userName);
        Task<UserAccount?> GetUserDetails(int id);
        Task<int> GetUserAccountByBankAccountNumber(int id);
    }    
}