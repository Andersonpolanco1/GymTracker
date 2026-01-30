using GymTracker.Enums;
using GymTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.Maintenance.WorkoutDays;

public class EditModel(GymTrackerDbContext context) : PageModel
{
  [BindProperty]
  public WorkoutDay? Day { get; set; } = null!;

  public List<Muscle> AllMuscles { get; set; } = [];
  public List<Exercise> StrengthExercises { get; set; } = [];

  [BindProperty] public int MuscleId { get; set; }
  [BindProperty] public int ExerciseId { get; set; }
  [BindProperty] public int PlannedSets { get; set; } = 3;
  [BindProperty] public int PlannedReps { get; set; } = 12;

  [BindProperty]
  public int CardioExerciseId { get; set; }  

  public List<Exercise> CardioExercises { get; set; } = [];


  public async Task<IActionResult> OnGetAsync(int id)
  {
    Day = await context.WorkoutDays
        .Include(d => d.Muscles).ThenInclude(m => m.Muscle)
        .Include(d => d.Exercises).ThenInclude(e => e.Exercise)
        .FirstOrDefaultAsync(d => d.Id == id);

    if (Day == null) return NotFound();

    AllMuscles = await context.Muscles.OrderBy(m => m.Name).ToListAsync();

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

  public async Task<IActionResult> OnPostAddMuscleAsync(int id)
  {
    if (!context.Set<WorkoutDayMuscle>()
        .Any(x => x.WorkoutDayId == id && x.MuscleId == MuscleId))
    {
      context.Add(new WorkoutDayMuscle
      {
        WorkoutDayId = id,
        MuscleId = MuscleId
      });

      await context.SaveChangesAsync();
    }

    return RedirectToPage(new { id });
  }

  public async Task<IActionResult> OnPostRemoveMuscleAsync(int id, int muscleId)
  {
    var item = await context.Set<WorkoutDayMuscle>()
        .FirstAsync(x => x.WorkoutDayId == id && x.MuscleId == muscleId);

    context.Remove(item);
    await context.SaveChangesAsync();

    return RedirectToPage(new { id });
  }

  public async Task<IActionResult> OnPostAddExerciseAsync(int id)
  {
    context.Add(new WorkoutDayExercise
    {
      WorkoutDayId = id,
      ExerciseId = ExerciseId,
      PlannedSets = PlannedSets,
      PlannedReps = PlannedReps
    });

    await context.SaveChangesAsync();
    return RedirectToPage(new { id });
  }

  public async Task<IActionResult> OnPostRemoveExerciseAsync(int id, int exId)
  {
    var ex = await context.Set<WorkoutDayExercise>().FindAsync(exId);
    if (ex != null)
    {
      context.Remove(ex);
      await context.SaveChangesAsync();
    }

    return RedirectToPage(new { id });
  }

  public async Task<IActionResult> OnPostAddCardioAsync(int id)
  {
    if (!context.Set<WorkoutDayExercise>()
        .Any(x => x.WorkoutDayId == id && x.ExerciseId == CardioExerciseId))
    {
      context.Add(new WorkoutDayExercise
      {
        WorkoutDayId = id,
        ExerciseId = CardioExerciseId,
        PlannedSets = 0, // Cardio no tiene series
        PlannedReps = 0
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
