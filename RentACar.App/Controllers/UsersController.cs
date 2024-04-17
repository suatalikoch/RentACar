using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentACar.App.Data;
using RentACar.App.Domain;
using RentACar.App.Models.Users;

namespace RentACar.App.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public UsersController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult All()
        {
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

            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            User user = new()
            {
                UserName = viewModel.UserName,
                Email = viewModel.Email,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                PIN = viewModel.PIN,
                PhoneNumber = viewModel.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, viewModel.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("All");
            }

            return View();
        }

        public async Task<IActionResult> Edit(string username, UserEditViewModel viewModel)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

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

            user.UserName = viewModel.UserName;
            user.Email = viewModel.Email;
            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, viewModel.Password);
            user.LastName = viewModel.LastName;
            user.PIN = viewModel.PIN;
            user.PhoneNumber = viewModel.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            if (result.Succeeded)
            {
                return RedirectToAction("All");
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

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

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            UserDeleteViewModel viewModel = new()
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(user);

            return RedirectToAction("All");
        }
    }
}
