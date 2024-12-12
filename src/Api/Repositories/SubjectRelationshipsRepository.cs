using UserService.Api.Data;
using UserService.Api.Models;
using UserService.Api.Repositories.Interfaces;

namespace UserService.Api.Repositories
{
    public class SubjectRelationshipsRepository : GenericRepository<SubjectRelationship>,
                                                ISubjectRelationshipsRepository
    {
        public SubjectRelationshipsRepository(DataContext context) : base(context)
        {
        }
    }
}