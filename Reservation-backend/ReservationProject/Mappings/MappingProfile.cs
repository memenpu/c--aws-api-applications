using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReservationProject.Models;

namespace ReservationProject.Mappings
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<User, LoginUserDto>();
            CreateMap<LoginUserDto, User>();
            CreateMap<User, EditUserDto>();
            CreateMap<EditUserDto, User>();
            CreateMap<Reservation, RoomDto>();
            CreateMap<RoomDto, Reservation>();
        }
    }
}
