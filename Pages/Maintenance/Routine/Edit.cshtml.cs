using GymTracker.Enums;
using GymTracker.Models;
using GymTracker;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.Maintenance.WorkoutDays
{
  public class EditModel : PageModel
  {
    private readonly GymTrackerDbContext context;

    public EditModel(GymTrackerDbContext context)
    {
      this.context = context;
    }

    [BindProperty]
    public WorkoutDay? Day { get; set; } = null!;

    public List<Exercise> StrengthExercises { get; set; } = [];
    public List<Exercise> CardioExercises { get; set; } = [];

    // SOLO UI (derivado)
    public List<string> Muscles { get; set; } = [];

    [BindProperty] public int ExerciseId { get; set; }
    [BindProperty] public int PlannedSets { get; set; } = 3;
    [BindProperty] public int PlannedReps { get; set; } = 12;

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

      Muscles = Day.Exercises
          .Where(e =>
              e.Exercise.Type == ExerciseType.Strength &&
              e.Exercise.Muscle != null)
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
}