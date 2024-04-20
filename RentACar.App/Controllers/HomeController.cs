using Microsoft.AspNetCore.Mvc;
using RentACar.App.Data;
using RentACar.App.Domain;
using RentACar.App.Models;
using RentACar.App.Models.Home;
using System.Diagnostics;
using System.Security.Claims; 

namespace RentACar.App.Controllers
{
    public class HomeController : Controller // Define HomeController class
    {
        private readonly ApplicationDbContext _context; // Define private field for database context

        public HomeController(ApplicationDbContext context) // Constructor to initialize database context
        {
            _context = context;
        }

        public IActionResult Index() // Action method for rendering the homepage
        {
            return View(); // Return the Index view
        }

        // Action method to handle errors
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Action method to handle search functionality
        public IActionResult Search(DateTime startDate, DateTime endDate)
        {
            // Validate input dates
            if (startDate <= DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "Start date must be today or after.");
            }

            if (startDate > endDate)
            {
                ModelState.AddModelError(string.Empty, "End date must be after start date.");
            }

            // If model state is invalid, return to Index view with error messages
            if (!ModelState.IsValid)
            {
                return View("Index");
            }

            // Retrieve unavailable rents for the specified date range
            var unavailableRents = _context.Rents
                .Where(r => !(r.EndDate < startDate || r.StartDate > endDate))
                .ToList();

            // Retrieve all cars from the database
            List<Car> allCars = _context.Cars.ToList();

            // Remove unavailable cars from the list
            foreach (var rent in unavailableRents)
            {
                allCars.RemoveAll(car => car.Id == rent.CarId);
            }

            // Create view model with available cars and date range
            var viewModel = new HomeViewModel
            {
                AvailableCars = allCars,
                StartDate = startDate,
                EndDate = endDate
            };

            // Return Index view with the view model
            return View("Index", viewModel);
        }

        // Action method to confirm a rent request
        public async Task<IActionResult> Rent(string carId, DateTime startDate, DateTime endDate)
        {
            // Find the selected car by ID
            var car = await _context.Cars.FindAsync(carId);

            // If car is not found, return NotFound result
            if (car == null)
            {
                return NotFound();
            }

            // Calculate rent duration
            TimeSpan rentDuration = endDate - startDate;

            // Create view model for rent confirmation
            RentConfirmViewModel viewModel = new()
            {
                CarId = car.Id,
                TenantId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year.ToString(),
                Passenger = car.Passenger.ToString(),
                Description = car.Description,
                RentPrice = car.RentPrice.ToString(),
                StartDate = startDate.ToString(),
                EndDate = endDate.ToString()
            };

            // Calculate total rent price
            decimal rentPrice = decimal.Parse(viewModel.RentPrice);
            decimal rentTotal = rentPrice * rentDuration.Days + rentPrice / 24 * rentDuration.Hours + rentPrice / 24 / 60 * rentDuration.Minutes;

            // Format rent duration and total
            viewModel.RentDuration = string.Concat(rentDuration.Days + " Days ", rentDuration.Hours + " Hours ", rentDuration.Minutes + " Minutes ");
            viewModel.RentTotal = rentTotal.ToString("0.00");

            // Return RentConfirm view with the view model
            return View("RentConfirm", viewModel);
        }

        // Action method to confirm a rent request
        public async Task<IActionResult> RentConfirm(string carId, string tenantId, string startDate, string endDate)
        {
            // Parse start and end dates
            DateTime startDateFormatted = DateTime.Parse(startDate);
            DateTime endDateFormatted = DateTime.Parse(endDate);

            // Create pending rent object
            PendingRent pendingRent = new()
            {
                CarId = carId,
                TenantId = tenantId,
                StartDate = startDateFormatted,
                EndDate = endDateFormatted
            };

            // Add pending rent to database and save changes
            await _context.PendingRents.AddAsync(pendingRent);
            await _context.SaveChangesAsync();

            // Return to Index view
            return View("Index");
        }
    }
}