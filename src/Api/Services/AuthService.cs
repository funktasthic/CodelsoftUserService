using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Services.Interfaces;
using Cubitwelve.Src.Exceptions;
using Microsoft.IdentityModel.Tokens;
using UserService.Api.Repositories.Interfaces;

namespace Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapperService _mapperService;
        private readonly IHttpContextAccessor _ctxAccesor;
        private readonly string _jwtSecret;

        public AuthService(IUnitOfWork unitOfWork,
        IConfiguration configuration,
        IMapperService mapperService,
        IHttpContextAccessor ctxAccesor
        )
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapperService = mapperService;
            _ctxAccesor = ctxAccesor;
            _jwtSecret = _configuration.GetSection("JwtSettings").GetValue<string>("Secret") ?? throw new InvalidJwtException("JWT_SECRET not found");

            Console.WriteLine("jwt secret authservice: " + _jwtSecret);
        }

        public string GetUserEmailInToken()
        {
            var httpUser = GetHttpUser();

            //Get Claims from JWT
            var userEmail = httpUser.FindFirstValue(ClaimTypes.Email) ??
                throw new UnauthorizedAccessException("Invalid user email in token");
            return userEmail;
        }

        public string GetUserRoleInToken()
        {
            var httpUser = GetHttpUser();

            //Get Claims from JWT
            var userRole = httpUser.FindFirstValue(ClaimTypes.Role) ??
                throw new UnauthorizedAccessException("Invalid role in token");
            return userRole;
        }

        private ClaimsPrincipal GetHttpUser()
        {
            //Check if the HttpContext is available to work with
            return (_ctxAccesor.HttpContext?.User) ??
                throw new UnauthorizedAccessException();
        }
    }
}