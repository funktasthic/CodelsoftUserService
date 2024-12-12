using Grpc.Core;

namespace Api.Services.Interfaces
{
    public interface IAuthService
    {
        public string GetUserEmailInToken(ServerCallContext context);
        public string GetUserRoleInToken(ServerCallContext context);
        public string CreateToken(string email, string role);
    }
}