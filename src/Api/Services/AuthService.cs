using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Services.Interfaces;
using Grpc.Core;
using Microsoft.IdentityModel.Tokens;
using UserService.Api.Exceptions;
using UserService.Api.Repositories.Interfaces;

namespace Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapperService _mapperService;
        private readonly string _jwtSecret;

        public AuthService(IUnitOfWork unitOfWork,
        IConfiguration configuration,
        IMapperService mapperService
        )
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapperService = mapperService;
            _jwtSecret = _configuration.GetSection("JwtSettings").GetValue<string>("Secret") ?? throw new InvalidJwtException("JWT_SECRET not found");
        }

        public string CreateToken(string email, string role)
        {
            var claims = new List<Claim>{
                new (ClaimTypes.Email, email),
                new (ClaimTypes.Role, role),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(60),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        public string GetUserEmailInToken(ServerCallContext context)
        {
            var httpUser = GetHttpUser(context);

            //Get Claims from JWT
            var userEmail = httpUser.FindFirstValue(ClaimTypes.Email) ??
                throw new UnauthorizedAccessException("Invalid user email in token");
            return userEmail;
        }

        public string GetUserRoleInToken(ServerCallContext context)
        {
            var httpUser = GetHttpUser(context);

            //Get Claims from JWT
            var userRole = httpUser.FindFirstValue(ClaimTypes.Role) ??
                throw new UnauthorizedAccessException("Invalid role in token");
            return userRole;
        }

        #region PRIVATE_METHODS

        private ClaimsPrincipal GetHttpUser(ServerCallContext context)
        {
            var httpContext = context.GetHttpContext();
            if (!httpContext.User.Identity?.IsAuthenticated ?? false)
                throw new UnauthorizedAccessException("User not authenticated");

            return httpContext.User;
        }

        #endregion
    }
}

// TOKEN: eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJ1c2VyQGV4YW1wbGUuY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoic3R1ZGVudCIsImV4cCI6MTczNDM0OTM3Mn0.8JHmP0HlB8qm-9efu-LN3oZ7bbiHDVbTtqXPNDDWhb4
