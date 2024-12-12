using AutoMapper;
using UserProto;
using UserService.Api.Models;

namespace UserService.Api.Extensions
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<Career, CareerDto>();
        }
    }
}