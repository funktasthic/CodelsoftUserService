using Grpc.Core;
using UserProto;

namespace Api.Services.Interfaces
{
    public interface IUsersService
    {
        Task<UserResponse> GetProfile(Empty request, ServerCallContext context);
        Task<UpdateUserProfileResponse> UpdateProfile(UpdateUserProfileDto updateUserProfileDto, ServerCallContext context);
        Task<UserDto> GetByEmail(string email);
    }
}
