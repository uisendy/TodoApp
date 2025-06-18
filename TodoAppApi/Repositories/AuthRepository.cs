using Microsoft.EntityFrameworkCore;
using TodoAppApi.Data;
using TodoAppApi.Entities;
using TodoAppApi.Interfaces;

namespace TodoAppApi.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
            => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
            =>  await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);


        public async Task<User?> GetUserByIdAsync(Guid userId)
            => await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        public async Task AddUserAsync(User user)
            => await _context.Users.AddAsync(user);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
