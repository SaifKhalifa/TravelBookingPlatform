using TravelBooking.Domain.Entities;
using TravelBooking.Infrastructure.Persistence;

namespace TravelBooking.Infrastructure.Seed;

public static class DbSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Cities.Any()) return; // if Already seeded

        // Seed RoomTypes
        var standard = new RoomType { Id = 1, Name = "Standard", Description = "Basic room with essential amenities." };
        var deluxe = new RoomType { Id = 2, Name = "Deluxe", Description = "Spacious room with premium features." };
        var suite = new RoomType { Id = 3, Name = "Suite", Description = "Top-tier room with luxury services." };
        context.RoomTypes.AddRange(standard, deluxe, suite);

        // Seed Discounts
        var weekendDeal = new Discount
        {
            Id = 1,
            Name = "Weekend Deal",
            Code = "WEEKEND15",
            Percentage = 15,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1)
        };

        var summerSaver = new Discount
        {
            Id = 2,
            Name = "Summer Saver",
            Code = "SUMMER20",
            Percentage = 20,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(3)
        };

        context.Discounts.AddRange(weekendDeal, summerSaver);

        // Seed Cities
        var city1 = new City { Name = "Paris", Country = "France", PostOffice = "75000" };
        var city2 = new City { Name = "Tokyo", Country = "Japan", PostOffice = "100-0001" };
        var city3 = new City { Name = "New York", Country = "USA", PostOffice = "10001" };
        context.Cities.AddRange(city1, city2, city3);

        // Seed Hotels
        var hotels = new List<Hotel>
        {
            new Hotel { Name = "Eiffel Grand", StarRate = 5, City = city1, Owner = "Pierre", Location = "Near Eiffel Tower" },
            new Hotel { Name = "Parisian Budget", StarRate = 3, City = city1, Owner = "Claire", Location = "Montmartre" },
            new Hotel { Name = "Tokyo Zen", StarRate = 4, City = city2, Owner = "Yuki", Location = "Shinjuku" },
            new Hotel { Name = "Sakura Stay", StarRate = 2, City = city2, Owner = "Hiro", Location = "Asakusa" },
            new Hotel { Name = "Manhattan View", StarRate = 5, City = city3, Owner = "Jake", Location = "Times Square" },
            new Hotel { Name = "NY Budget Inn", StarRate = 2, City = city3, Owner = "Sara", Location = "Harlem" }
        };
        context.Hotels.AddRange(hotels);

        // Seed Rooms
        var rooms = new List<Room>();
        int roomTypeToggle = 1;
        int discountToggle = 1;
        foreach (var hotel in hotels)
        {
            rooms.Add(new Room
            {
                Hotel = hotel,
                RoomNumber = $"R{hotel.Name.Replace(" ", "").Substring(0, 3).ToUpper()}01",
                Adults = 2,
                Children = 1,
                PricePerNight = 120,
                IsAvailable = true,
                RoomTypeId = roomTypeToggle,
                DiscountId = discountToggle
            });

            rooms.Add(new Room
            {
                Hotel = hotel,
                RoomNumber = $"R{hotel.Name.Replace(" ", "").Substring(0, 3).ToUpper()}02",
                Adults = 3,
                Children = 2,
                PricePerNight = 180,
                IsAvailable = true,
                RoomTypeId = roomTypeToggle == 3 ? 1 : roomTypeToggle + 1,
                DiscountId = null
            });

            roomTypeToggle = roomTypeToggle == 3 ? 1 : roomTypeToggle + 1;
            discountToggle = discountToggle == 2 ? 1 : 2;
        }

        context.Rooms.AddRange(rooms);

        context.SaveChanges();
    }
}