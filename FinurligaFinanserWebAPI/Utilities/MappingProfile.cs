using AutoMapper;
using Entity;
using FinurligaFinanserWebAPI.DtoModels.BankAccountDTOs;
using FinurligaFinanserWebAPI.DtoModels.UserAccountDTOs;
using Infrastructure.Helpers;

namespace FinurligaFinanserWebAPI.Utilities
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {            
            CreateMap<UserAccount, UserAccountConfirmationDTO>()
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "User account " +src.UserName+" successfully created."));

            CreateMap<LoginUserDTO, UserAccount>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(dto => dto.UserName))
                .ForMember(u => u.PasswordSalt, opt => opt.MapFrom(dto => PasswordHasher.GenerateSalt()))
                .ForMember(u => u.PasswordHash, opt => opt.MapFrom(dto => PasswordHasher.HashPassword(dto.Password, PasswordHasher.GenerateSalt())))
                .ForMember(u => u.FirstName, opt => opt.MapFrom(dto => string.Empty))
                .ForMember(u => u.LastName, opt => opt.MapFrom(dto => string.Empty));

            CreateMap<PostBankAccountDTO, BankAccount>().ReverseMap();
            CreateMap<BankAccountDTO, BankAccount>().ReverseMap();

            CreateMap<UserAccount, UserAccountDetailsDTO>()
                .ForMember(u => u.id, opt => opt.MapFrom(dto => dto.Id))
                .ForMember(u => u.firstName, opt => opt.MapFrom(dto => dto.FirstName))
                .ForMember(u => u.lastName, opt => opt.MapFrom(dto => dto.LastName));
        }
    }
}