using Grpc.Core;
using UserProto;

namespace Api.Services.Interfaces
{
    public interface IUsersService
    {
        Task<UserResponse> GetProfile(Empty request, ServerCallContext context);
    }
}