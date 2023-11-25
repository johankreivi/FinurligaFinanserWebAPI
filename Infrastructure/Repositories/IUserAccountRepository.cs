
using Entity;

namespace Infrastructure.Repositories
{
    public interface IUserAccountRepository
    {
        Task<List<UserAccount>> GetAllUserAccountsAsync();
    }
}
