using GymTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.WorkoutSessions
{
  public class DetailsModel(GymTrackerDbContext ctx) : PageModel
  {

    // ===== DATOS PRINCIPALES =====
    public WorkoutSession? Session { get; set; }

    public List<StrengthExerciseSummary> StrengthSummary { get; set; } = [];
    public List<CardioSession> Cardio { get; set; } = [];

    // ===== TOTALES =====
    public decimal TotalVolume { get; set; }
    public int TotalCardioMinutes { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
      Session = await ctx.WorkoutSessions
          .Include(s => s.Sets)
              .ThenInclude(es => es.Exercise)
          .FirstOrDefaultAsync(s => s.Id == id);

      if (Session == null)
        return NotFound();

      // ===== FUERZA: RESUMEN POR EJERCICIO =====
      StrengthSummary = [.. Session.Sets
          .GroupBy(s => s.Exercise.Name)
          .Select(g => new StrengthExerciseSummary
          {
            ExerciseName = g.Key,
            Sets = g.Count(),
            TotalReps = g.Sum(x => x.Reps),
            TotalVolume = g.Sum(x => x.Volume)
          })
          .OrderBy(e => e.ExerciseName)];

      TotalVolume = StrengthSummary.Sum(s => s.TotalVolume);

      // ===== CARDIO =====
      Cardio = await ctx.CardioSessions
          .Include(c => c.Exercise)
          .Where(c => c.WorkoutSessionId == id)
          .OrderBy(c => c.Exercise.Name)
          .ToListAsync();

      TotalCardioMinutes = Cardio.Sum(c => c.DurationMinutes);

      return Page();
    }
  }

  public class StrengthExerciseSummary
  {
    public string ExerciseName { get; set; } = "";
    public int Sets { get; set; }
    public int TotalReps { get; set; }
    public decimal TotalVolume { get; set; }
  }
}
