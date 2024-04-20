using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Create(UserCreateViewModel viewModel)
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

        // Action to display the edit user form
        public async Task<IActionResult> Edit(string username, UserEditViewModel viewModel)
        {
            // Find the user to be edited
            var user = await _userManager.FindByNameAsync(username);

            // If user is not found, return a not found error
            if (user == null)
            {
                return NotFound();
            }

            // If form data is invalid, return the edit user form with the existing data pre-filled
            if (!ModelState.IsValid)
            {
                viewModel.UserName = user.UserName;
                viewModel.Email = user.Email;
                viewModel.Password = user.PasswordHash;
                viewModel.ConfirmPassword = user.PasswordHash;
                viewModel.FirstName = user.FirstName;
                viewModel.LastName = user.LastName;
                viewModel.PIN = user.PIN;
                viewModel.PhoneNumber = user.PhoneNumber;

                return View(viewModel);
            }

            // Update user data with the new values from the form
            user.UserName = viewModel.UserName;
            user.Email = viewModel.Email;
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, viewModel.Password);
            user.LastName = viewModel.LastName;
            user.PIN = viewModel.PIN;
            user.PhoneNumber = viewModel.PhoneNumber;

            // Attempt to update the user in the database
            var result = await _userManager.UpdateAsync(user);

            // If update is successful, redirect to the list of all users
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(All));
            }

            // If update fails, return to the edit user form
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
