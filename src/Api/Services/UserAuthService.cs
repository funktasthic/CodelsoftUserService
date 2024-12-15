using Api.Services.Interfaces;
using UserService.Api.Exceptions;
using UserService.Api.Repositories.Interfaces;
using UserAuthProto;
using Grpc.Core;
using UserService.Api.Models;
using UserService.Api.Common.Constants;
using System.Security.Authentication;
using MassTransit;

namespace Api.Services
{
    public class UserAuthService : UserAuthProto.UserAuthService.UserAuthServiceBase, IUserAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapperService _mapperService;
        private readonly IAuthService _authService;
        private readonly IPublishEndpoint _publishEndpoint;

        public UserAuthService(IUnitOfWork unitOfWork,
            IMapperService mapperService,
            IAuthService authService,
            IPublishEndpoint publishEndpoint)
        {
            _authService = authService;
            _unitOfWork = unitOfWork;
            _mapperService = mapperService;
            _publishEndpoint = publishEndpoint;
        }

        public override async Task<RegisterResponseDto> Register(RegisterStudentDto registerStudentDto, ServerCallContext context)
        {
            // Validate Email and RUT
            await ValidateEmailAndRUT(registerStudentDto.Email, registerStudentDto.Rut);

            var role = (await _unitOfWork.RolesRepository.Get(r => r.Name == RolesEnum.STUDENT)).FirstOrDefault();
            if (role is null)
            {
                throw new InternalErrorException("Role not found");
            }

            var career = await _unitOfWork.CareersRepository.GetByID(registerStudentDto.CareerId);
            if (career is null)
            {
                throw new EntityNotFoundException($"Career with ID: {registerStudentDto.CareerId} not found");
            }

            var mappedUser = _mapperService.Map<RegisterStudentDto, User>(registerStudentDto);
            mappedUser.RoleId = role.Id;
            mappedUser.CareerId = career.Id;
            mappedUser.IsEnabled = true;

            var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            mappedUser.HashedPassword = BCrypt.Net.BCrypt.HashPassword(registerStudentDto.Password, salt);

            var createdUser = await _unitOfWork.UsersRepository.Insert(mappedUser);

            // Generate the token
            var token = _authService.CreateToken(createdUser.Email, createdUser.Role.Name);

            // Mapping the response
            var response = _mapperService.Map<User, RegisterResponseDto>(createdUser);
            response.Token = token;

            // Publish the event
            await _publishEndpoint.Publish(mappedUser.ToString() ?? string.Empty);

            return response;
        }

        public override async Task<Empty> UpdatePassword(UpdatePasswordDto updatePasswordDto, ServerCallContext context)
        {
            var userEmail = _authService.GetUserEmailInToken(context);
            var user = await _unitOfWork.UsersRepository.GetByEmail(userEmail)
                ?? throw new EntityNotFoundException($"User with email: {userEmail} does not exist");

            // Verifying the current password
            var verifyPassword = BCrypt.Net.BCrypt.Verify(updatePasswordDto.CurrentPassword, user.HashedPassword);
            if (!verifyPassword)
                throw new InvalidCredentialException("Invalid Current Password");

            // Updating the password
            var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            user.HashedPassword = BCrypt.Net.BCrypt.HashPassword(updatePasswordDto.Password, salt);

            // Save the updated user
            await _unitOfWork.UsersRepository.Update(user);

            // Return empty response on success
            return new Empty();
        }

        #region PRIVATE_METHODS
        private async Task ValidateEmailAndRUT(string email, string rut)
        {
            var user = await _unitOfWork.UsersRepository.GetByEmail(email);
            if (user is not null)
            {
                throw new DuplicateUserException("Email already in use");
            }

            user = await _unitOfWork.UsersRepository.GetByRut(rut);
            if (user is not null)
            {
                throw new DuplicateUserException("RUT already in use");
            }
        }
        #endregion
    }
}

