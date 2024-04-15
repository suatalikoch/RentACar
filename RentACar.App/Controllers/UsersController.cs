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
            if (!ModelState.IsValid)
            {
                return View();
            }

            User userFromDb = new()
            {
                UserName = bindingModel.UserName,
                Email = bindingModel.Email,
                FirstName = bindingModel.FirstName,
                LastName = bindingModel.LastName,
                PIN = bindingModel.PIN,
                PhoneNumber = bindingModel.PhoneNumber
            };

            var result = await _userManager.CreateAsync(userFromDb, bindingModel.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("All");
            }

            return View();
        }

        public async Task<IActionResult> Edit(string username, UserEditBindingModel bindingModel)
        {
            if (!ModelState.IsValid)
            {
                return View(bindingModel);
            }


            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            user.UserName = bindingModel.UserName;
            user.Email = bindingModel.Email;
            user.FirstName = bindingModel.FirstName;
            user.LastName = bindingModel.LastName;
            user.PIN = bindingModel.PIN;
            user.PhoneNumber = bindingModel.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            if (result.Succeeded)
            {
                return RedirectToAction("All");
            }

            return View(bindingModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string username)
        {
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

        [HttpGet]
        public async Task<IActionResult> Delete(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            
            if (user == null)
            {
                return NotFound();
            }

            UserDeleteBindingModel bindingModel = new()
            {
                UserName = user.UserName,
                FirstName= user.FirstName,
                LastName= user.LastName,
            };

            return View(bindingModel);
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
