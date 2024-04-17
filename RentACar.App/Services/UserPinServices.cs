using Microsoft.EntityFrameworkCore;
using RentACar.App.Data;
using RentACar.App.Domain;

namespace RentACar.App.Services
{
    public class UserPINServices
    {
        private readonly ApplicationDbContext _context;

        public UserPINServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> FindByPINAsync(string PIN)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PIN == PIN);
        }
    }
}
