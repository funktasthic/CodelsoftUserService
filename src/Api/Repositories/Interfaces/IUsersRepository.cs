using UserService.Api.Models;

namespace UserService.Api.Repositories.Interfaces
{
    public interface IUsersRepository : IGenericRepository<User>
    {


        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of users</returns>
        public Task<List<User>> GetAll();

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <returns>User or null</returns>
        public Task<User?> GetByEmail(string email);

        /// <summary>
        /// Get user by rut
        /// </summary>
        /// <param name="rut">The RUT of the user</param>
        /// <returns>User or null</returns>
        public Task<User?> GetByRut(string rut);

        /// <summary>
        /// Get user progress by user id
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <returns>Progress of the User</returns>
        public Task<List<UserProgress>?> GetProgressByUser(int userId);

        public Task<bool> AddProgress(List<UserProgress> progress);

        public Task<bool> RemoveProgress(List<UserProgress> progress, int userId);
    }
}