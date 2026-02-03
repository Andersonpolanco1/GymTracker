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
      var sessions = await ctx.WorkoutSessions
          .Include(s => s.WorkoutDay)
          .Include(s => s.PerformedExercises)
          .OrderByDescending(s => s.Date)
          .ToListAsync();

      Sessions = sessions.Select(s =>
      {
        var strength = s.PerformedExercises
            .OfType<StrengthSet>();

        var cardio = s.PerformedExercises
            .OfType<TimedSet>();

        return new WorkoutSessionRow
        {
          Id = s.Id,
          Date = s.Date,
          DayName = Utils.Utilities.GetDayNameInSpanishCapitalized(
              s.WorkoutDay.DayOfWeek),

          StrengthSets = strength.Count(),

          TotalVolume = strength.Sum(x => x.Reps!.Value * x.Weight!.Value),

          CardioMinutes = (int)cardio
              .Sum(c => c.Duration.TotalMinutes)
        };
      }).ToList();
    }
  }

  public class WorkoutSessionRow
  {
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string DayName { get; set; } = "";

    public int StrengthSets { get; set; }
    public decimal TotalVolume { get; set; }
    public int CardioMinutes { get; set; }
  }
}
