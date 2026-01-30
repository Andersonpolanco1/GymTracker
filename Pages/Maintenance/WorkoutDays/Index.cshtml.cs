using GymTracker.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.Maintenance.WorkoutDays;

public class IndexModel(GymTrackerDbContext context) : PageModel
{
  public List<WorkoutDay> Days { get; set; } = [];

  public async Task OnGetAsync()
  {
    Days = await context.WorkoutDays
        .Include(d => d.Muscles)
            .ThenInclude(dm => dm.Muscle)
        .Include(d => d.Exercises)
            .ThenInclude(de => de.Exercise)
        .OrderBy(d => d.DayOfWeek)
        .ToListAsync();
  }
}
