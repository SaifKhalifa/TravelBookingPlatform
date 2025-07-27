using TravelBooking.Domain.Entities;
using TravelBooking.Infrastructure.Persistence;

namespace TravelBooking.Infrastructure.Seed;

public static class DbSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Cities.Any()) return; // Already seeded

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

        // Seed Rooms (2 per hotel)
        var rooms = new List<Room>();
        foreach (var hotel in hotels)
        {
            rooms.Add(new Room
            {
                Hotel = hotel,
                RoomNumber = $"R{hotel.Id}01",
                Adults = 2,
                Children = 1,
                PricePerNight = 120,
                IsAvailable = true
            });

            rooms.Add(new Room
            {
                Hotel = hotel,
                RoomNumber = $"R{hotel.Id}02",
                Adults = 3,
                Children = 2,
                PricePerNight = 180,
                IsAvailable = true
            });
        }

        context.Rooms.AddRange(rooms);

        context.SaveChanges();
    }
}
