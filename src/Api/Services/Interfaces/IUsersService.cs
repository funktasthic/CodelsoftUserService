using Grpc.Core;
using UserProto;

namespace Api.Services.Interfaces
{
    public interface IUsersService
    {
        public Task<UserResponse> GetProfile(Empty request, ServerCallContext context);
        public Task<UpdateUserProfileResponse> UpdateProfile(UpdateUserProfileDto updateUserProfileDto, ServerCallContext context);
        public Task<UserDto> GetByEmail(string email);
    }
}
