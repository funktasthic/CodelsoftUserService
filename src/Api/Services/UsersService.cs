using Api.Services.Interfaces;
using Google.Protobuf.Collections;
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
            try
            {
                var userEmail = _authService.GetUserEmailInToken(context);
                var getByEmail = await GetByEmail(userEmail);
                var response = new UserResponse { User = getByEmail };
                return response;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        public override async Task<UpdateUserProfileResponse> UpdateProfile(UpdateUserProfileDto updateUserProfileDto, ServerCallContext context)
        {
            try
            {
                var userEmail = _authService.GetUserEmailInToken(context);
                var user = await GetUserByEmail(userEmail);

                user.Name = updateUserProfileDto.Name ?? user.Name;
                user.FirstLastName = updateUserProfileDto.FirstLastName ?? user.FirstLastName;
                user.SecondLastName = updateUserProfileDto.SecondLastName ?? user.SecondLastName;

                var updatedUser = await _unitOfWork.UsersRepository.Update(user);
                var updatedUserDto = _mapperService.Map<User, UpdateUserProfileDto>(updatedUser);

                return new UpdateUserProfileResponse { User = updatedUserDto };
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        public override async Task<UserProgressResponse> GetUserProgress(Empty request, ServerCallContext context)
        {
            try
            {
                var userId = await GetUserIdByToken(context);
                var userProgress = await _unitOfWork.UsersRepository.GetProgressByUser(userId);

                if (userProgress == null || !userProgress.Any())
                    throw new EntityNotFoundException("No progress data found for the user.");

                var progressDtos = _mapperService.Map<IEnumerable<UserProgress>, RepeatedField<UserProgressDto>>(userProgress);

                return new UserProgressResponse { UserProgress = { progressDtos } };
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        #region PRIVATE_METHODS

        private async Task<UserDto> GetByEmail(string email)
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

        private async Task<int> GetUserIdByToken(ServerCallContext context)
        {
            var userEmail = _authService.GetUserEmailInToken(context);
            var user = await _unitOfWork.UsersRepository.GetByEmail(userEmail)
                        ?? throw new EntityNotFoundException("User not found");
            return user.Id;
        }

        #endregion

        #region EXCEPTION_HANDLING

        private void HandleException(Exception ex)
        {
            switch (ex)
            {
                case EntityNotFoundException:
                    throw new RpcException(new Status(StatusCode.NotFound, ex.Message), ex.Message);
                case UnauthorizedAccessException:
                    throw new RpcException(new Status(StatusCode.Unauthenticated, ex.Message), ex.Message);
                default:
                    throw new RpcException(new Status(StatusCode.Internal, ex.Message), ex.Message);
            }
        }

        #endregion
    }
}
