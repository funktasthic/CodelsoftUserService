using UserService.Api.Data;
using UserService.Api.Models;
using UserService.Api.Repositories.Interfaces;

namespace UserService.Api.Repositories
{
    public class SubjectsRepository : GenericRepository<Subject>, ISubjectsRepository
    {
        public SubjectsRepository(DataContext context) : base(context)
        {
        }
    }
}