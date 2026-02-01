using GymTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.Maintenance.WorkoutDays
{
  public class IndexModel(
    GymTrackerDbContext context,
    UserManager<ApplicationUser> userManager) : PageModel
  {
    public List<DayItem> Days { get; set; } = [];

    [BindProperty]
    public List<DayOfWeek> SelectedDays { get; set; } = [];

    public async Task OnGetAsync()
    {
      var userId = userManager.GetUserId(User)!;

      var userDays = await context.WorkoutDays
        .Where(x => x.UserId == userId)
        .Select(x => x.DayOfWeek)
        .ToListAsync();

      Days = Enum.GetValues<DayOfWeek>()
        .Where(d => d != DayOfWeek.Sunday)
        .Select(d => new DayItem
        {
          Day = d,
          IsSelected = userDays.Contains(d)
        })
        .ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      var userId = userManager.GetUserId(User)!;

      var existingDays = await context.WorkoutDays
        .Where(x => x.UserId == userId)
        .ToListAsync();

      context.WorkoutDays.RemoveRange(existingDays);

      foreach (var day in SelectedDays)
      {
        context.WorkoutDays.Add(new WorkoutDay
        {
          UserId = userId,
          DayOfWeek = day
        });
      }

      await context.SaveChangesAsync();
      return RedirectToPage();
    }

    public class DayItem
    {
      public DayOfWeek Day { get; set; }
      public bool IsSelected { get; set; }
    }
  }
}
