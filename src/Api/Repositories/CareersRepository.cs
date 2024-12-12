using UserService.Api.Data;
using UserService.Api.Models;
using UserService.Api.Repositories.Interfaces;

namespace UserService.Api.Repositories
{
    public class CareersRepository : GenericRepository<Career>, ICareersRepository
    {
        public CareersRepository(DataContext context) : base(context)
        {
        }
    }
}