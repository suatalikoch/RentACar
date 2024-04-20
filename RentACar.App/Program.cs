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
            builder.Services.AddScoped<UserPINServices>();

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
                    var cars = new List<Car>
                    {
                        new() {
                            Brand = "Mercedes",
                            Model = "S class",
                            Year = 2007,
                            Passenger = 5,
                            ImageLink = "https://media.autoexpress.co.uk/image/private/s--X-WVjvBW--/f_auto,t_content-image-full-desktop@1/v1563184167/autoexpress/2017/11/dsc_9993.jpg",
                            Description = "Lorem ipsum dolor sit amet.",
                            RentPrice = 99.999m
                        },
                        new() {
                            Brand = "Chevrolet",
                            Model = "Camaro",
                            Year = 2022,
                            Passenger = 4,
                            ImageLink="https://hips.hearstapps.com/hmg-prod/images/2019-chevrolet-camaro-2-0t-1le-6mt-106-1539790974.jpg?crop=0.603xw:0.738xh;0.306xw,0.262xh&resize=768:*",
                            Description = "Classic American muscle car with modern performance.",
                            RentPrice = 80.00m
                        },
                        new() {
                            Brand = "BMW",
                            Model = "X5",
                            Year = 2020,
                            Passenger = 5,
                            ImageLink="https://cdn-images.fleetnews.co.uk/thumbs/1400x1000/web-clean/1/bmw-x5-and-x6-2023/p90489754-highres.jpg",
                            Description = "Luxury SUV with premium features and comfort.",
                            RentPrice = 120.00m
                        },
                        new() {
                            Brand = "Mercedes-Benz",
                            Model = "E-Class",
                            Year = 2019,
                            Passenger = 5,
                            ImageLink="https://hips.hearstapps.com/hmg-prod/images/2024-mercedes-benz-e-class-exterior-105-6446a2cb7003d.jpg?crop=0.671xw:0.753xh;0.135xw,0.247xh&resize=768:*",
                            Description = "Executive sedan known for its comfort and elegance.",
                            RentPrice = 110.00m
                        },
                        new() {
                            Brand = "Jeep",
                            Model = "Wrangler",
                            Year = 2021,
                            Passenger = 4,
                            ImageLink="https://hips.hearstapps.com/hmg-prod/images/2024-jeep-wrangler-4xe-rubicon-x-504-65b27d09dc3b6.jpg?crop=0.606xw:0.453xh;0.204xw,0.348xh&resize=1200:*",
                            Description = "Iconic off-road SUV for adventure seekers.",
                            RentPrice = 90.00m
                        },
                        new() {
                            Brand = "Lexus",
                            Model = "LS",
                            Year = 2023,
                            Passenger = 5,
                            ImageLink = @"https://hips.hearstapps.com/hmg-prod/images/2020-lexus-ls500h-205-1586827841.jpg?crop=0.702xw:0.785xh;0.130xw,0.215xh&resize=768:*",
                            Description = "Luxurious and refined sedan.",
                            RentPrice = 200.00m
                        },
                        new() {
                            Brand = "Cadillac",
                            Model = "CT6",
                            Year = 2022,
                            Passenger = 5,
                            ImageLink = "https://www.cnet.com/a/img/resize/13b3706a0d436975c0666276b399439be7dd73a1/hub/2020/05/13/b688efe2-b633-47a8-ad5d-f5ca17aa1f8f/ogi1-2020-cadillac-ct6-017-copy.jpg?auto=webp&fit=crop&height=675&width=1200",
                            Description = "Sleek and sophisticated luxury sedan.",
                            RentPrice = 210.00m
                        },
                        new() {
                            Brand = "Porsche",
                            Model = "Panamera",
                            Year = 2023,
                            Passenger = 4,
                            ImageLink = "https://carwow-uk-wp-2.imgix.net/New-Porsche-Panamera-lead-1.png?auto=format&cs=tinysrgb&fit=crop&h=800&ixlib=rb-1.1.0&q=60&w=1600",
                            Description = "Luxurious sports sedan with exhilarating performance.",
                            RentPrice = 280.00m
                        },
                        new() {
                            Brand = "Ferrari",
                            Model = "Portofino",
                            Year = 2023,
                            Passenger = 4,
                            ImageLink = "https://car-images.bauersecure.com/wp-images/12459/ferrari_portofino_50.jpg",
                            Description = "Stylish convertible grand tourer with blistering performance.",
                            RentPrice = 350.00m
                        },
                        new() {
                            Brand = "Lamborghini",
                            Model = "Huracan",
                            Year = 2023,
                            Passenger = 4,
                            ImageLink = "https://stimg.cardekho.com/images/carexteriorimages/630x420/Lamborghini/Huracan-EVO/10643/1690010999692/front-left-side-47.jpg",
                            Description = "Aggressive supercar with cutting-edge technology and unmatched performance",
                            RentPrice = 400.00m
                        },
                        new() {
                            Brand = "Lamborghini",
                            Model = "Aventador",
                            Year = 2023,
                            Passenger = 2,
                            ImageLink = "https://www.insidehook.com/wp-content/uploads/2022/11/lamborghini-aventador-ultimae-driving.jpg?fit=1500%2C1000s",
                            Description = "Iconic hypercar with extreme performance and jaw-dropping design.",
                            RentPrice = 550.00m
                        },
                        new() {
                            Brand = "Lamborghini",
                            Model = "Urus",
                            Year = 2023,
                            Passenger = 4,
                            ImageLink = "https://images.drive.com.au/driveau/image/upload/c_fill,f_auto,g_auto,h_675,q_auto:best,w_1200/v1/cms/uploads/giz3atasiffnrc1fbbhx",
                            Description = "Luxury SUV combining Lamborghini's DNA with practicality and comfort.",
                            RentPrice = 450.00m
                        }
                    };

                    foreach (var car in cars)
                    {
                        await context.Cars.AddAsync(car);
                    }

                    await context.SaveChangesAsync();
                }

                if (!context.Rents.Any())
                {
                    var rent = new Rent()
                    {
                        CarId = context.Cars.FirstOrDefaultAsync().Result.Id,
                        TenantId = context.Users.FirstOrDefaultAsync().Result.Id,
                        StartDate = DateTime.ParseExact("13/04/2024", "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        EndDate = DateTime.ParseExact("20/04/2024", "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    };

                    await context.Rents.AddAsync(rent);
                    await context.SaveChangesAsync();
                }
            }

            app.Run();
        }
    }
}
