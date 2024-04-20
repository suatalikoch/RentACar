using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentACar.App.Data;
using RentACar.App.Domain;
using RentACar.App.Models.Users;

namespace RentACar.App.Controllers
{
    // Restricts access to only users with "Administrator" role
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        // Constructor injection to get access to ApplicationDbContext and UserManager
        public UsersController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Action to display all users
        public IActionResult All()
        {
            // Retrieve user information from the database and map it to a view model
            List<UserAllViewModel> users = _context.Users
                .Select(user => new UserAllViewModel
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PIN = user.PIN,
                    PhoneNumber = user.PhoneNumber
                }).ToList();

            // Pass the list of users to the view for display
            return View(users);
        }

        // Action to display the create user form
        public IActionResult Create()
        {
            return View();
        }

        // Action to handle user creation form submission
        [HttpPost]
        public async Task<IActionResult> Create(UserCreateEditViewModel viewModel)
        {
            // Validate form data
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Create a new user object with the provided data
            User user = new()
            {
                UserName = viewModel.UserName,
                Email = viewModel.Email,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                PIN = viewModel.PIN,
                PhoneNumber = viewModel.PhoneNumber
            };

            // Attempt to create the user in the database
            var result = await _userManager.CreateAsync(user, viewModel.Password);

            // If user creation is successful, redirect to the list of all users
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(All));
            }

            // If user creation fails, return to the create user form
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new UserCreateEditViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PIN = user.PIN,
                PhoneNumber = user.PhoneNumber
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string username, UserCreateEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(username);

                if (user == null)
                {
                    return NotFound();
                }

                user.UserName = viewModel.UserName;
                user.Email = viewModel.Email;
                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, viewModel.Password);
                user.FirstName = user.FirstName;
                user.LastName = viewModel.LastName;
                user.PIN = viewModel.PIN;
                user.PhoneNumber = viewModel.PhoneNumber;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    Console.WriteLine(e.Message);
                }

                return RedirectToAction(nameof(All));
            }

            return View(viewModel);
        }

        // Action to display user details
        [HttpGet]
        public async Task<IActionResult> Details(string username)
        {
            // Find the user whose details are to be displayed
            var user = await _userManager.FindByNameAsync(username);

            // If user is not found, return a not found error
            if (user == null)
            {
                return NotFound();
            }

            // Map user data to a view model for display
            var viewModel = new UserDetailsViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PIN = user.PIN,
                PhoneNumber = user.PhoneNumber,
                PasswordHash = user.PasswordHash
            };

            // Pass the view model to the view for display
            return View(viewModel);
        }

        // Action to display the delete user confirmation page
        [HttpGet]
        public async Task<IActionResult> Delete(string username)
        {
            // Find the user to be deleted
            var user = await _userManager.FindByNameAsync(username);

            // If user is not found, return a not found error
            if (user == null)
            {
                return NotFound();
            }

            // Map user data to a view model for display
            UserDeleteViewModel viewModel = new()
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            // Pass the view model to the view for display
            return View(viewModel);
        }

        // Action to handle user deletion confirmation
        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(string username)
        {
            // Find the user to be deleted
            var user = await _userManager.FindByNameAsync(username);

            // If user is not found, return a not found error
            if (user == null)
            {
                return NotFound();
            }

            // Delete the user from the database
            await _userManager.DeleteAsync(user);

            // Redirect to the list of all users after deletion
            return RedirectToAction(nameof(All));
        }
    }
}
