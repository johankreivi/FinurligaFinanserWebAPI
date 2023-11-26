using Entity;

namespace Infrastructure.Repositories
{
    public interface IUserAccountRepository
    {
        Task<List<UserAccount>> GetAllUserAccountsAsync(int take);
        Task<UserAccount> CreateUserAccount(CreateUserAccountDTO dto);
        Task<UserAccount> GetOneUser(int id);
    }    
}
