using Microsoft.AspNetCore.Identity;
using AutoMapper;
using InheritedIdentityRole.Dto;

namespace InheritedIdentityRole;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserRegistrationDto, IdentityUser>();
    }
}