using GymTracker.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.WorkoutSessions
{
  public class IndexModel(GymTrackerDbContext ctx) : PageModel
  {
    public List<WorkoutSessionRow> Sessions { get; set; } = [];

    public async Task OnGetAsync()
    {
      // 1 Traemos sesiones con sets
      var sessions = await ctx.WorkoutSessions
          .Include(s => s.WorkoutDay)
          .Include(s => s.Sets)
          .ToListAsync();

      // 2 Calculamos todo en memoria
      Sessions = sessions
          .Select(s => new WorkoutSessionRow
          {
            Id = s.Id,
            Date = s.Date,
            DayName = Utils.Utilities.GetDayNameInSpanishCapitalized( s.WorkoutDay.DayOfWeek),

            StrengthSets = s.Sets.Count,

            TotalVolume = s.Sets.Sum(x => x.Reps * x.Weight),

            CardioMinutes = ctx.CardioSessions
                  .Where(c => c.WorkoutSessionId == s.Id)
                  .Sum(c => c.DurationMinutes)
          })
          .OrderByDescending(s => s.Date)
          .ToList();
    }
  }

  public class WorkoutSessionRow
  {
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string DayName { get; set; } = "";

    public int StrengthSets { get; set; }
    public decimal TotalVolume { get; set; }
    public int CardioMinutes { get; set; }
  }
}
