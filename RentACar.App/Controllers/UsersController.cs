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
        private UserManager<User> _userManager;

        public UsersController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult All()
        {
            List<UserAllViewModel> users = _context.Users
                .Select(userFromDb => new UserAllViewModel
                {
                    UserName = userFromDb.UserName,
                    Email = userFromDb.Email,
                    FirstName = userFromDb.FirstName,
                    LastName = userFromDb.LastName,
                    PIN = userFromDb.PIN,
                    PhoneNumber = userFromDb.PhoneNumber
                }).ToList();

            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateBindingModel bindingModel)
        {
            if (ModelState.IsValid)
            {
                User userFromDb = new User
                {
                    UserName = bindingModel.UserName,
                    Email = bindingModel.Email,
                    FirstName = bindingModel.FirstName,
                    LastName = bindingModel.LastName,
                    PIN = bindingModel.PIN,
                    PhoneNumber = bindingModel.PhoneNumber
                };

                var result = await _userManager.CreateAsync(userFromDb, bindingModel.Password);

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return RedirectToAction("All");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string username, UserEditBindingModel bindingModel)
        {
            if (username != bindingModel.UserName)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(username);

                if (user == null)
                {
                    return NotFound();
                }

                user.Email = bindingModel.Email;
                user.FirstName = bindingModel.FirstName;
                user.LastName = bindingModel.LastName;
                user.PIN = bindingModel.PIN;
                user.PhoneNumber = bindingModel.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("All");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(bindingModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string username)
        {
            if (username == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            var model = new UserDetailsViewModel
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

            return View(model);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("All");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }
    }
}
