using GymTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.MigrateExercises
{
  public class IndexModel(GymTrackerDbContext context) : PageModel
  {
    public List<MigratedExerciseVm> MigratedExercises { get; set; } = [];

    public async Task<IActionResult> OnPostAsync()
    {

      if (context.PerformedExercises.Any())
      {
        ModelState.AddModelError("", "La migración ya fue ejecutada.");
        return Page();
      }



      var sets = await context.ExerciseSets
        .Include(x => x.Exercise)
        .ToListAsync();

      foreach (var set in sets)
      {
        var strength = new StrengthSet
        {
          WorkoutSessionId = set.WorkoutSessionId,
          ExerciseId = set.ExerciseId,
          Reps = set.Reps,
          Weight = set.Weight,
          RIR = set.RIR,
          RestSeconds = set.RestSeconds,
          Order = set.SetNumber
        };

        context.PerformedExercises.Add(strength);

        MigratedExercises.Add(new MigratedExerciseVm
        {
          WorkoutSessionId = set.WorkoutSessionId,
          ExerciseName = set.Exercise.Name,
          Type = "Strength",
          Detail = $"{set.Reps} reps x {set.Weight} lb"
        });
      }

      // Migrar CardioSession ? TimedExerciseSession
      var cardioSessions = await context.CardioSessions
        .Include(x => x.Exercise)
        .ToListAsync();

      foreach (var cardio in cardioSessions)
      {
        var timed = new TimedExerciseSession
        {
          WorkoutSessionId = cardio.WorkoutSessionId,
          ExerciseId = cardio.ExerciseId,
          Duration = TimeSpan.FromMinutes(cardio.DurationMinutes),
          DistanceKm = cardio.DistanceKm,
          Calories = cardio.Calories,
          AvgHeartRate = cardio.AvgHeartRate
        };

        context.PerformedExercises.Add(timed);

        MigratedExercises.Add(new MigratedExerciseVm
        {
          WorkoutSessionId = cardio.WorkoutSessionId,
          ExerciseName = cardio.Exercise.Name,
          Type = "Cardio",
          Detail = $"{cardio.DurationMinutes} min"
        });
      }

      await context.SaveChangesAsync();

      return Page();
    }
  }

  public class MigratedExerciseVm
  {
    public int WorkoutSessionId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
  }


}
