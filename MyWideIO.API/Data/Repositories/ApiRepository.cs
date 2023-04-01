using Microsoft.EntityFrameworkCore;
using MyVideIO.Data;
using MyWideIO.API.Data.IRepositories;

namespace MyWideIO.API.Data.Repositories
{
    public class ApiRepository: IApiRepository
    {
        protected ApplicationDbContext Context { get; set; }

        private bool SaveChanges()
        {
            var saved = Context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool AddUser(/*parameters*/)
        {
            // do sth;
            return SaveChanges();
        }
    }
}
