using AutoMapper;
using BlackoutManager.DATA.Dtos;
using BlackoutManager.DATA.Models;

namespace BlackoutManager.DATA.AutoMapper;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        //user
        CreateMap<User, UserRegisterDto>().ReverseMap();
        CreateMap<User, UserLoginDto>().ReverseMap();
    }
}
