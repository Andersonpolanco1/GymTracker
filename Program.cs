using GymTracker;
using GymTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{  options.Conventions.AuthorizeFolder("/");

  options.Conventions.AllowAnonymousToPage("/Login");
});

builder.Services.AddDbContext<GymTrackerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GymTrackerDb")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
  options.Password.RequiredLength = 6;
  options.Password.RequireNonAlphanumeric = false;
  options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<GymTrackerDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
  options.LoginPath = "/Login";         
  options.AccessDeniedPath = "/Login";
  options.ExpireTimeSpan = TimeSpan.FromHours(8); 
  options.SlidingExpiration = true;
});

builder.Services.AddAuthorization();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

// -----------------------
// Crear usuario administrador si no existe y su rutina
// -----------------------
using (var scope = app.Services.CreateScope())
{
  var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

  string adminEmail = "andersonpolancocontreras@gmail.com";
  string adminPassword = "Andy1993";

  if (await userManager.FindByEmailAsync(adminEmail) == null)
  {
    var user = new ApplicationUser
    {
      UserName = adminEmail,
      Email = adminEmail,
      DisplayName = "Anderson"
    };

    await userManager.CreateAsync(user, adminPassword);
    await userManager.AddClaimAsync(user, new Claim("DisplayName", user.DisplayName));
  }
}

app.MapRazorPages();

app.Run();
