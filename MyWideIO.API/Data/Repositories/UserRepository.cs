using MyVideIO.Data;
using MyWideIO.API.Data.IRepositories;

namespace MyWideIO.API.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
