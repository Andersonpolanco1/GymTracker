using GymTracker.Enums;
using GymTracker.ViewModels.RoutineModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.Routine;

public class IndexModel(GymTrackerDbContext context) : PageModel
{
  public List<WeeklyRoutineVm> Routine { get; set; } = [];

  public async Task OnGetAsync()
  {
    var days = await context.WorkoutDays
        .Include(d => d.Exercises)
            .ThenInclude(e => e.Exercise)
                .ThenInclude(e => e.Muscle)
        .OrderBy(d => d.DayOfWeek)
        .ToListAsync();

    Routine = days.Where(d => d.IsActive).Select(day => new WeeklyRoutineVm
    {
      DayOfWeek = day.DayOfWeek,
      DayName = Utils.Utilities.GetDayNameInSpanishCapitalized(day.DayOfWeek),

      Muscles = day.Exercises
            .Where(e =>
                e.Exercise.Type == ExerciseType.Strength &&
                e.Exercise.Muscle != null)
            .GroupBy(e => e.Exercise.Muscle!)
            .OrderBy(g => g.Key.Name)
            .Select(g => new MuscleRoutineVm
            {
              MuscleName = g.Key.Name,

              Exercises = g.Select(e => new RoutineExerciseVm
              {
                Name = e.Exercise.Name,
                Sets = e.PlannedSets,
                Reps = e.PlannedReps,
                Type = e.Exercise.Type
              }).ToList()
            })
            .ToList(),

      Cardio = day.Exercises
            .Where(e => e.Exercise.Type == ExerciseType.Cardio)
            .Select(e => new RoutineExerciseVm
            {
              Name = e.Exercise.Name,
              Type = ExerciseType.Cardio
            })
            .ToList()
    }).ToList();
  }
}
