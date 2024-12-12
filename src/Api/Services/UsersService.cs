using Api.Services.Interfaces;
using Grpc.Core;
using UserProto;
using UserService.Api.Models;
using UserService.Api.Repositories.Interfaces;

namespace Api.Services
{
    public class UsersService : UserProto.UsersService.UsersServiceBase, IUsersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapperService _mapperService;
        private readonly IAuthService _authService;

        // Constructor que recibe las dependencias necesarias
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
                // Obtener el correo del usuario desde el token JWT
                var userEmail = _authService.GetUserEmailInToken();

                // Recuperar el usuario desde la base de datos utilizando el correo electrónico
                var user = await _unitOfWork.UsersRepository.GetByEmail(userEmail);

                // Verificar si el usuario existe
                if (user == null)
                {
                    // Si no existe, lanzar una excepción gRPC
                    throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
                }

                // Mapear el modelo User a UserDto utilizando AutoMapper
                var userDto = _mapperService.Map<User, UserDto>(user);

                // Crear y devolver la respuesta con el UserDto
                return new UserResponse
                {
                    User = userDto
                };
            }
            catch (Exception ex)
            {
                // Manejo de excepciones en caso de error
                throw new RpcException(new Status(StatusCode.Internal, $"Error retrieving user profile: {ex.Message}"));
            }
        }
    }
}
