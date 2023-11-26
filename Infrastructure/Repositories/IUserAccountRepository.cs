using Entity;

namespace Infrastructure.Repositories
{
    public interface IUserAccountRepository
    {
        Task<List<UserAccount>> GetAllUserAccountsAsync(int take);
        Task<UserAccount> CreateUserAccount(UserAccount userAccount);
        Task<UserAccount> GetOneUser(int id);
    }    
}
