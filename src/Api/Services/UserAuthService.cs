using Api.Services.Interfaces;
using UserService.Api.Exceptions;
using UserService.Api.Repositories.Interfaces;
using UserAuthProto;
using Grpc.Core;
using UserService.Api.Models;
using UserService.Api.Common.Constants;

namespace Api.Services
{
    public class UserAuthService : UserAuthProto.UserAuthService.UserAuthServiceBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapperService _mapperService;
        private readonly IAuthService _authService;

        public UserAuthService(IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IMapperService mapperService,
            IAuthService authService)
        {
            _authService = authService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapperService = mapperService;
        }

        public override async Task<RegisterResponseDto> Register(RegisterStudentDto registerStudentDto, ServerCallContext context)
        {
            try
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

                // Complete mapping
                var response = _mapperService.Map<User, RegisterResponseDto>(createdUser);
                response.Token = token;

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in registration: {ex.Message}");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        private async Task ValidateEmailAndRUT(string email, string rut)
        {
            Console.WriteLine("Validating email and RUT...");

            var user = await _unitOfWork.UsersRepository.GetByEmail(email);
            if (user is not null)
            {
                Console.WriteLine("Email already in use.");
                throw new DuplicateUserException("Email already in use");
            }

            user = await _unitOfWork.UsersRepository.GetByRut(rut);
            if (user is not null)
            {
                Console.WriteLine("RUT already in use.");
                throw new DuplicateUserException("RUT already in use");
            }

            Console.WriteLine("Email and RUT are available.");
        }
    }
}
