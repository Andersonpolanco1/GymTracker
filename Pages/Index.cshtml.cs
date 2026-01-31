using GymTracker.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages
{
  public class IndexModel(GymTrackerDbContext context) : PageModel
  {

    // Datos para la vista
    public DateTime Today { get; set; }
    public string DayName { get; set; } = string.Empty;

    public List<string> Muscles { get; set; } = [];
    public List<ExerciseItem> Exercises { get; set; } = [];
    public List<CardioItem> Cardio { get; set; } = [];

    public async Task OnGetAsync()
    {
      // Fecha actual
      Today = DateTime.Today;

      // Nombre del día en español
      DayName = Utils.Utilities.GetDayNameInSpanishCapitalized(Today.DayOfWeek);

      // Buscar el WorkoutDay según el día actual
      var workoutDay = await context.WorkoutDays
          //.Include(w => w.Muscles)
          //    .ThenInclude(m => m.Muscle)
          .Include(w => w.Exercises)
              .ThenInclude(e => e.Exercise)
                  .ThenInclude(ex => ex.Muscle)
          .FirstOrDefaultAsync(w => w.DayOfWeek == Today.DayOfWeek);

      if (workoutDay == null)
        return;

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
            Reps = x.PlannedReps
          })
          .OrderBy(x => x.Muscle)
          .ToList();

      // Cardio (si aplica ese día)
      Cardio = workoutDay.Exercises
          .Where(x => x.Exercise.Type == ExerciseType.Cardio)
          .Select(x => new CardioItem
          {
            Name = x.Exercise.Name,
            Minutes = 20 // valor por defecto o configurable
          })
          .ToList();
    }

    public class ExerciseItem
    {
      public string Muscle { get; set; } = string.Empty;
      public string Name { get; set; } = string.Empty;
      public int Sets { get; set; }
      public int Reps { get; set; }
    }

    public class CardioItem
    {
      public string Name { get; set; } = string.Empty;
      public int Minutes { get; set; }
    }

  }


}
