using Api.Services.Interfaces;
using Grpc.Core;
using UserProto;
using UserService.Api.Exceptions;
using UserService.Api.Models;
using UserService.Api.Repositories.Interfaces;

namespace Api.Services
{
    public class UsersService : UserProto.UsersService.UsersServiceBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapperService _mapperService;
        private readonly IAuthService _authService;

        public UsersService(IUnitOfWork unitOfWork, IMapperService mapperService, IAuthService authService)
        {
            _unitOfWork = unitOfWork;
            _mapperService = mapperService;
            _authService = authService;
        }

        public override async Task<UserResponse> GetProfile(Empty request, ServerCallContext context)
        {
            var userEmail = _authService.GetUserEmailInToken(context);
            var getByEmail = await GetByEmail(userEmail);
            var response = new UserResponse();
            response.User = getByEmail;
            return response;
        }

        #region PRIVATE_METHODS

        public async Task<UserDto> GetByEmail(string email)
        {
            var user = await GetUserByEmail(email);
            return _mapperService.Map<User, UserDto>(user);
        }

        private async Task<User> GetUserByEmail(string email)
        {
            var user = await _unitOfWork.UsersRepository.GetByEmail(email)
                                        ?? throw new EntityNotFoundException($"User with email: {email} not found");
            return user;
        }

        #endregion
    }
}
