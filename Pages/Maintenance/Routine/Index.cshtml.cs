using GymTracker.Enums;
using GymTracker.ViewModels.MaintenanceModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.Maintenance.Routine;

public class IndexModel(GymTrackerDbContext context) : PageModel
{
  public List<WorkoutDaySummaryVm> Days { get; set; } = [];

  public async Task OnGetAsync()
  {
    var daysRaw = await context.WorkoutDays
        .Include(d => d.Exercises)
            .ThenInclude(e => e.Exercise)
                .ThenInclude(e => e.Muscle)
        .OrderBy(d => d.DayOfWeek)
        .ToListAsync();

    Days = daysRaw.Where(d=> d.IsActive)
      .Select(d => new WorkoutDaySummaryVm
    {
      Id = d.Id,
      DayOfWeek = d.DayOfWeek,

      // Fuerza agrupada por músculo
      MuscleGroups = d.Exercises
            .Where(e =>
                e.Exercise.Type == ExerciseType.Strength &&
                e.Exercise.Muscle != null)
            .GroupBy(e => e.Exercise.Muscle!.Name)
            .OrderBy(g => g.Key)
            .Select(g => new MuscleGroupVm
            {
              MuscleName = g.Key,
              Exercises = g.Select(e => new ExerciseItemVm
              {
                Name = e.Exercise.Name,
                Sets = e.PlannedSets,
                Reps = e.PlannedReps
              }).ToList()
            })
            .ToList(),

      CardioExercises = d.Exercises
            .Where(e => e.Exercise.Type == ExerciseType.Cardio)
            .Select(e => new CardioItemVm
            {
              Name = e.Exercise.Name
            })
            .ToList()
    }).ToList();
  }
}