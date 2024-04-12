using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentACar.App.Data;
using RentACar.App.Domain;
using RentACar.App.Models.Users;

namespace RentACar.App.Controllers
{
    //[Authorize(Roles = "Administrator")]
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

        public IActionResult Index()
        {
            return View();
        }
    }
}
