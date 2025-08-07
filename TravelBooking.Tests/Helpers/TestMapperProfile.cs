using AutoMapper;
using TravelBooking.Application.DTOs;
using TravelBooking.Domain.Entities;

namespace TravelBooking.Tests.Helpers;

public class TestMapperProfile : Profile
{
    public TestMapperProfile()
    {
        CreateMap<Booking, BookingDto>();
        CreateMap<BookingCreateDto, Booking>();
        CreateMap<Room, RoomDto>();
    }
}