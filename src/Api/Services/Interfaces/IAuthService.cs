namespace Api.Services.Interfaces
{
    public interface IAuthService
    {
        public string GetUserEmailInToken();
        public string GetUserRoleInToken();
    }
}