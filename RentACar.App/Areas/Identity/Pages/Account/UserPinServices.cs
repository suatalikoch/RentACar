using Microsoft.EntityFrameworkCore;
using RentACar.App.Data;
using RentACar.App.Domain;

namespace RentACar.App.Areas.Identity.Pages.Account
{
    public class UserPinServices
    {
        private readonly ApplicationDbContext _context;

        public UserPinServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> FindByPINAsync(string pin)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PIN == pin);
        }
    }
}
