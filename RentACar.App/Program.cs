using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RentACar.App.Data;
using RentACar.App.Domain;
using RentACar.App.Services;
using System.Globalization;

namespace RentACar.App
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            builder.Services.AddControllersWithViews();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;
            });

            // Add Razor Pages services
            builder.Services.AddRazorPages();

            // Register UserPinService
            builder.Services.AddScoped<UserPinServices>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var roles = new[]
                {
                    "Administrator",
                    "User"
                };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }

            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                if (await userManager.FindByEmailAsync("admin@gmail.com") == null)
                {
                    var user = new User()
                    {
                        UserName = "admin",
                        Email = "admin@gmail.com",
                        FirstName = "Admin",
                        LastName = "Admin",
                        PIN = "0000000000",
                        PhoneNumber = "0000000"
                    };

                    await userManager.CreateAsync(user, "admin");
                    await userManager.AddToRoleAsync(user, "Administrator");
                }

                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (!context.Cars.Any())
                {
                    var car = new Car()
                    {
                        Brand = "Mersodes",
                        Model = "S class",
                        Year = 2007,
                        Passenger = 5,
                        Description = "Lorem ipsum dolor sit amet.",
                        RentPrice = 99.999m,
                        Renter = context.Users.FirstOrDefault().UserName,
                        RenterId = context.Users.FirstOrDefault().Id
                    };

                    await context.Cars.AddAsync(car);
                    await context.SaveChangesAsync();
                }

                if (!context.Rents.Any())
                {
                    var rent = new Rent()
                    {
                        CarId = context.Cars.FirstOrDefault().Id,
                        TenantId = context.Users.FirstOrDefault().Id,
                        StartDate = DateTime.ParseExact("13/04/2024", "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        EndDate = DateTime.ParseExact("20/04/2024", "dd/MM/yyyy", CultureInfo.InvariantCulture)
                    };

                    await context.Rents.AddAsync(rent);
                    await context.SaveChangesAsync();
                }
            }

            app.Run();
        }
    }
}
