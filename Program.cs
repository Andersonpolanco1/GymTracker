using GymTracker;
using GymTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
  options.Conventions.AuthorizeFolder("/");
  options.Conventions.AllowAnonymousToPage("/Login");
  options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
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
  options.AccessDeniedPath = "/AccessDenied";
  options.ExpireTimeSpan = TimeSpan.FromHours(8);
  options.SlidingExpiration = true;
});

builder.Services.AddAuthorizationBuilder()
  .AddPolicy("AdminOnly", policy =>
      policy.RequireRole("Admin"));

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



app.MapRazorPages();

app.Run();
