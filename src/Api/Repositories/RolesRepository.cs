using UserService.Api.Data;
using UserService.Api.Models;
using UserService.Api.Repositories.Interfaces;

namespace UserService.Api.Repositories
{
    public class RolesRepository : GenericRepository<Role>, IRolesRepository
    {
        public RolesRepository(DataContext context) : base(context)
        {
        }
    }
}