using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.App.Data;
using RentACar.App.Models.Rents;
using System.Security.Claims;

namespace RentACar.App.Controllers
{
    [Authorize]
    public class MyRentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor to inject ApplicationDbContext
        public MyRentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action method to display all rents
        public IActionResult All()
        {
            // Assuming you have access to the current user's ID
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Function to retrieve current user's ID

            // Retrieve all rents along with associated car and tenant information
            List<RentAllViewModel> myRents = _context.Rents
                .Where(rent => rent.TenantId == currentUserId) // Filter by current user's ID
                .Join(_context.Cars,
                    rent => rent.CarId,
                    car => car.Id,
                    (rent, car) => new { rent, car })
                .Join(_context.Users,
                    combined => combined.rent.TenantId,
                    user => user.Id,
                    (combined, user) => new RentAllViewModel
                    {
                        Id = combined.rent.Id,
                        CarId = combined.rent.CarId,
                        CarBrand = combined.car.Brand,
                        CarModel = combined.car.Model,
                        StartDate = combined.rent.StartDate,
                        EndDate = combined.rent.EndDate,
                        TenantId = combined.rent.TenantId,
                        TenantFirstName = user.FirstName,
                        TenantLastName = user.LastName
                    }).ToList();

            return View(myRents);
        }
    }
}
