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
                .ForMember(x=>x.From, x=>x.MapFrom(x=>x.Sender.FullName))
                .ForMember(x=>x.Avatar, x=>x.MapFrom(x=>x.Sender.Avatar))
                .ForMember(x=>x.Room,x=>x.MapFrom(x=>x.Room.RoomName))
                .ForMember(x=>x.Content,x=>x.MapFrom(x=>x.Content))
                .ReverseMap();
            CreateMap<Room, RoomVM>()
               .ReverseMap();
        }
    }
}
