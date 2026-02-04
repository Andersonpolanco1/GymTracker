using GymTracker.Enums;
using GymTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages
{
  public class IndexModel(GymTrackerDbContext context, UserManager<ApplicationUser> userManager) : PageModel
  {

    public DateTime Today { get; set; }
    public string DayName { get; set; } = string.Empty;
    public bool HasWorkoutDay { get; set; } = true;
    public bool HasExercises { get; set; }

    public List<string> Muscles { get; set; } = [];
    public List<ExerciseItem> Exercises { get; set; } = [];
    public List<CardioItem> Cardio { get; set; } = [];

    public async Task OnGetAsync()
    {

      var userId = userManager.GetUserId(User)!;

      // Fecha actual
      Today = Utils.Utilities.NowRD();

      // Nombre del día en español
      DayName = Utils.Utilities.GetDayNameInSpanishCapitalized(Today.DayOfWeek);

      // Buscar el WorkoutDay según el día actual
      var workoutDay = await context.WorkoutDays
        .Where(d => d.IsActive && d.UserId == userId)
          .Include(w => w.Exercises)
              .ThenInclude(e => e.Exercise)
                  .ThenInclude(ex => ex.Muscle)
          .FirstOrDefaultAsync(w => w.DayOfWeek == Today.DayOfWeek);

      if (workoutDay == null)
      {
        HasWorkoutDay = false;
        return;
      }

      // Músculos del día
      Muscles = workoutDay.Exercises
          .Where(e => e.Exercise.Muscle != null)
          .Select(e => e.Exercise.Muscle!.Name)
          .Distinct()
          .OrderBy(m => m)
          .ToList();


      // Ejercicios de fuerza
      Exercises = workoutDay.Exercises
          .Where(x => x.Exercise.Type == ExerciseType.Strength)
          .Select(x => new ExerciseItem
          {
            Muscle = x.Exercise.Muscle!.Name,
            Name = x.Exercise.Name,
            Sets = x.PlannedSets,
            Reps = x.PlannedReps,
            Minutes = x.PlannedDurationSeconds.HasValue ? x.PlannedDurationSeconds.Value: null,
            ImagePath = x.Exercise.ImagePath,
            Notes = x.Exercise.Notes,
          })
          .OrderBy(x => x.Muscle)
          .ToList();

      // Cardio (si aplica ese día)
      Cardio = workoutDay.Exercises
          .Where(x => x.Exercise.Type == ExerciseType.Cardio)
          .Select(x => new CardioItem
          {
            Name = x.Exercise.Name,
            Minutes = 20, // valor por defecto o configurable,
            ImagePath = x.Exercise.ImagePath,
            Notes = x.Exercise.Notes,
          })
          .ToList();

      HasExercises = Exercises.Count != 0 || Cardio.Count != 0;
    }

    public class ExerciseItem
    {
      public string Muscle { get; set; } = string.Empty;
      public string Name { get; set; } = string.Empty;
      public int Sets { get; set; }
      public int? Reps { get; set; }
      public int? Minutes { get; set; }
      public string? ImagePath { get; set; }
      public string? Notes { get; set; }


    }

    public class CardioItem
    {
      public string Name { get; set; } = string.Empty;
      public int Minutes { get; set; }
      public string? ImagePath { get; set; }
      public string? Notes { get; set; }
    }

  }


}
