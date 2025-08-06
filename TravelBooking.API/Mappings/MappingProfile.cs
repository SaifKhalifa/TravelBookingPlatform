using AutoMapper;
using TravelBooking.Application.DTOs;
using TravelBooking.Domain.Entities;

namespace TravelBooking.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ENTITY -> DTO
        CreateMap<User, UserDto>();

        CreateMap<Room, RoomDto>()
            .ForMember(dest => dest.RoomType, opt => opt.MapFrom(src => src.RoomType!.Name))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount != null ? src.Discount.Name : null));

        CreateMap<Booking, BookingDto>()
            .ForMember(dest => dest.Hotel, opt => opt.MapFrom(src => src.Room!.Hotel!.Name))
            .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.Room!.RoomNumber))
            .ForMember(dest => dest.RoomType, opt => opt.MapFrom(src => src.Room!.RoomType!.Name))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Room != null && src.Room.Discount != null ? src.Room.Discount.Name : null)); // CS8072 Fix

        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User!.Name))
            .ForMember(dest => dest.Hotel, opt => opt.MapFrom(src => src.Hotel!.Name));

        CreateMap<Hotel, HotelWithRoomsDto>()
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City!.Name));

        // DTO -> ENTITY
        CreateMap<CreateHotelDto, Hotel>();
        CreateMap<CreateRoomDto, Room>();
        CreateMap<CreateRoomTypeDto, RoomType>();
        CreateMap<CreateDiscountDto, Discount>();
    }
}
