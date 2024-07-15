using AutoMapper;
using VehiclesControl.Domain.Entities;
using VehiclesControl.Domain.Input;
using VehiclesControl.Domain.Outs;

namespace VehiclesControl.Data.MapperProfile
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<UserRequest, User>();
            CreateMap<User, UserRequest>();

            CreateMap<UserResponse, User>();
            CreateMap<User, UserResponse>();

            CreateMap<CarRequest, Car>();
            CreateMap<Car, CarRequest>();

            CreateMap<CarResponse, Car>();
            CreateMap<Car, CarResponse>();

            CreateMap<BusRequest, Bus>();
            CreateMap<Bus, BusRequest>();

            CreateMap<BusResponse, Bus>();
            CreateMap<Bus, BusResponse>();

            CreateMap<BoatRequest, Boat>();
            CreateMap<Boat, BoatRequest>();

            CreateMap<BoatResponse, Boat>();
            CreateMap<Boat, BoatResponse>();
        }
    }
}
