using Microsoft.EntityFrameworkCore;
using TravelBooking.Application.Services.Interfaces.IRepository;
using TravelBooking.Domain.Entities;
using TravelBooking.Infrastructure.Persistence;

namespace TravelBooking.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;

    public ReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasReviewedAsync(int userId, int hotelId) =>
        await _context.Reviews.AnyAsync(r => r.UserId == userId && r.HotelId == hotelId && !r.IsDeleted);

    public async Task<Hotel?> GetHotelByIdAsync(int hotelId) =>
        await _context.Hotels.FindAsync(hotelId);

    public async Task<User?> GetUserByIdAsync(int userId) =>
        await _context.Users.FindAsync(userId);

    public async Task AddReviewAsync(Review review) =>
        await _context.Reviews.AddAsync(review);

    public async Task<List<Review>> GetReviewsByHotelAsync(int hotelId) =>
        await _context.Reviews
            .Where(r => r.HotelId == hotelId && !r.IsDeleted)
            .Include(r => r.User)
            .Include(r => r.Hotel)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<List<Review>> GetReviewsByUserAsync(int userId) =>
        await _context.Reviews
            .Where(r => r.UserId == userId)
            .Include(r => r.Hotel)
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<Review?> GetReviewByIdAsync(int reviewId) =>
        await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
