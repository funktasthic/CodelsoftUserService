using Grpc.Core;
using UserAuthProto;

namespace Api.Services.Interfaces
{
    public interface IUserAuthService
    {
        Task<Empty> UpdatePassword(UpdatePasswordDto updatePasswordDto, ServerCallContext context);
        Task<RegisterResponseDto> Register(RegisterStudentDto registerStudentDto, ServerCallContext context);
    }
}