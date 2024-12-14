using UserAuthProto;
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
            CreateMap<User, UpdateUserProfileDto>();
            CreateMap<Career, CareerDto>();
            CreateMap<UserProgress, UserProgressDto>();
            CreateMap<User, RegisterResponseDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name))
            .ForMember(dest => dest.Career, opt => opt.MapFrom(src => src.Career.Name));
            CreateMap<RegisterStudentDto, User>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.FirstLastName, opt => opt.MapFrom(src => src.FirstLastName))
            .ForMember(dest => dest.SecondLastName, opt => opt.MapFrom(src => src.SecondLastName))
            .ForMember(dest => dest.RUT, opt => opt.MapFrom(src => src.Rut))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.CareerId, opt => opt.MapFrom(src => src.CareerId))
            .ForMember(dest => dest.HashedPassword, opt => opt.Ignore());
        }
    }
}