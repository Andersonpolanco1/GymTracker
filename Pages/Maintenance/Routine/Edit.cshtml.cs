using GymTracker.Enums;
using GymTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.Maintenance.Routine
{
  public class EditModel(GymTrackerDbContext context) : PageModel
  {
    [BindProperty]
    public WorkoutDay? Day { get; set; } = null!;

    // Listas separadas de ejercicios ya filtradas por tipo
    public List<WorkoutDayExercise> StrengthExercisesInDay { get; set; } = [];
    public List<WorkoutDayExercise> CardioExercisesInDay { get; set; } = [];

    public List<Exercise> StrengthExercises { get; set; } = [];
    public List<Exercise> CardioExercises { get; set; } = [];

    public List<string> Muscles { get; set; } = [];

    [BindProperty] public int ExerciseId { get; set; }
    [BindProperty] public int PlannedSets { get; set; } = 3;
    [BindProperty] public int? PlannedReps { get; set; }
    [BindProperty] public int? PlannedDuration { get; set; }

    [BindProperty] public int CardioExerciseId { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
      Day = await context.WorkoutDays
          .Include(d => d.Exercises)
              .ThenInclude(e => e.Exercise)
                  .ThenInclude(e => e.Muscle)
          .FirstOrDefaultAsync(d => d.Id == id);

      if (Day == null)
        return NotFound();

      // Separar ejercicios por tipo
      StrengthExercisesInDay = Day.Exercises
          .Where(e => e.Exercise.Type == ExerciseType.Strength)
          .ToList();

      CardioExercisesInDay = Day.Exercises
          .Where(e => e.Exercise.Type == ExerciseType.Cardio)
          .ToList();

      Muscles = StrengthExercisesInDay
          .Where(e => e.Exercise.Muscle != null)
          .Select(e => e.Exercise.Muscle!.Name)
          .Distinct()
          .OrderBy(m => m)
          .ToList();

      StrengthExercises = await context.Exercises
          .Where(e => e.Type == ExerciseType.Strength)
          .OrderBy(e => e.Name)
          .ToListAsync();

      CardioExercises = await context.Exercises
          .Where(e => e.Type == ExerciseType.Cardio)
          .OrderBy(e => e.Name)
          .ToListAsync();

      return Page();
    }

    // ================= FUERZA =================
    public async Task<IActionResult> OnPostAddExerciseAsync(int id)
    {
      if (!context.Set<WorkoutDayExercise>().Any(x => x.WorkoutDayId == id && x.ExerciseId == ExerciseId))
      {
        context.Add(new WorkoutDayExercise
        {
          WorkoutDayId = id,
          ExerciseId = ExerciseId,
          PlannedSets = PlannedSets,
          PlannedReps = PlannedReps,
          PlannedDurationSeconds = PlannedDuration
        });

        await context.SaveChangesAsync();
      }

      return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostRemoveExerciseAsync(int id, int exId)
    {
      var dayExercise = await context.Set<WorkoutDayExercise>()
          .FirstOrDefaultAsync(e => e.Id == exId && e.WorkoutDayId == id);

      if (dayExercise != null)
      {
        context.Remove(dayExercise);
        await context.SaveChangesAsync();
      }

      return RedirectToPage(new { id });
    }

    // ================= CARDIO =================
    public async Task<IActionResult> OnPostAddCardioAsync(int id)
    {
      if (!context.Set<WorkoutDayExercise>()
          .Any(x => x.WorkoutDayId == id && x.ExerciseId == CardioExerciseId))
      {
        context.Add(new WorkoutDayExercise
        {
          WorkoutDayId = id,
          ExerciseId = CardioExerciseId,
          PlannedSets = 0,
          PlannedReps = 0,
          PlannedDurationSeconds = PlannedDuration 
        });

        await context.SaveChangesAsync();
      }

      return RedirectToPage(new { id });
    }


    public async Task<IActionResult> OnPostRemoveCardioAsync(int id, int exId)
    {
      var cardio = await context.Set<WorkoutDayExercise>()
          .FirstOrDefaultAsync(e => e.WorkoutDayId == id && e.Id == exId);

      if (cardio != null)
      {
        context.Remove(cardio);
        await context.SaveChangesAsync();
      }

      return RedirectToPage(new { id });
    }
  }
}
