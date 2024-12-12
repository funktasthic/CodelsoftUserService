using Grpc.Core;
using UserProto;

namespace Api.Services.Interfaces
{
    public interface IUsersService
    {
        public Task<UserResponse> GetProfile(Empty request, ServerCallContext context);
        public Task<UserResponse> GetByEmail(string email);
    }
}