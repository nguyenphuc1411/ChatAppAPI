using AutoMapper;
using ChatAppAPI.Data.Entities;
using ChatAppAPI.Models;

namespace ChatAppAPI.Mappings
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Message, MessageVM>()
                .ForMember(x=>x.FullName, x=>x.MapFrom(x=>x.Sender.FullName))
                .ForMember(x => x.Email, x => x.MapFrom(x => x.Sender.Email))
                .ForMember(x=>x.Avatar, x=>x.MapFrom(x=>x.Sender.Avatar))
                .ForMember(x=>x.RoomId,x=>x.MapFrom(x=>x.Room.Id))
                .ForMember(x=>x.Content,x=>x.MapFrom(x=>x.Content))
                .ReverseMap();
            CreateMap<Room, RoomVM>()
                .ForMember(x=>x.AdminName,x=>x.MapFrom(x=>x.Admin.FullName))
                 .ForMember(x => x.Avatar, x => x.MapFrom(x => x.Admin.Avatar))
               .ReverseMap();

            CreateMap<ManageUser, UserVM>()
                .ForMember(x => x.FullName, x => x.MapFrom(x => x.FullName))
                .ForMember(x => x.Avatar, x => x.MapFrom(x => x.Avatar))
                .ForMember(x=>x.Email,x=>x.MapFrom(x=>x.Email));
        }
    }
}
