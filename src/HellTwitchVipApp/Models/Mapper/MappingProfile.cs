using AutoMapper;
using HellTwitchVipApp.Data.Entities.Models;
using HellTwitchVipApp.Models.Dto;

namespace HellTwitchVipApp.Models.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<GiftSubscriptionGiver, GiverDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.UserName))
                .ForMember(d => d.Count, opt => opt.MapFrom(s => s.GiftCount))
                .ForMember(d => d.IsVip, opt => opt.MapFrom(s => s.IsVip))
                .ReverseMap();
        }
    }
}
