using AutoMapper;
using Entity;
using FinurligaFinanserWebAPI.DtoModels;

namespace FinurligaFinanserWebAPI.Utilities
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {            
            CreateMap<UserAccount, UserAccountConfirmationDTO>()
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "User account " +src.UserName+" successfully created."));
        }
    }
}