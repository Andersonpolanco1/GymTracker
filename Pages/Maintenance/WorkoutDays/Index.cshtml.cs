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

    public async Task OnGetAsync()
    {
      var userId = userManager.GetUserId(User)!;

      var activeDays = await context.WorkoutDays
        .Where(x => x.UserId == userId && x.IsActive)
        .Select(x => x.DayOfWeek)
        .ToListAsync();

      Days = Enum.GetValues<DayOfWeek>()
        .Select(d => new DayItem
        {
          Day = d,
          IsSelected = activeDays.Contains(d)
        })
        .ToList();
    }


    public async Task<IActionResult> OnPostToggleAsync(DayOfWeek day)
    {
      var userId = userManager.GetUserId(User)!;

      var existing = await context.WorkoutDays
        .FirstOrDefaultAsync(x =>
          x.UserId == userId &&
          x.DayOfWeek == day);

      if (existing is null)
      {
        context.WorkoutDays.Add(new WorkoutDay
        {
          UserId = userId,
          DayOfWeek = day,
          IsActive = true
        });
      }
      else
      {
        var wasActive = existing.IsActive;
        existing.IsActive = !existing.IsActive;

        if (wasActive && !existing.IsActive)
        {
          var dayExercises = await context.WorkoutDayExercises
            .Where(we => we.WorkoutDayId == existing.Id)
            .ToListAsync();

          if (dayExercises.Count != 0)
          {
            context.WorkoutDayExercises.RemoveRange(dayExercises);
          }
        }
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
