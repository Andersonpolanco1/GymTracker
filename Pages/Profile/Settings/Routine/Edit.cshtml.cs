using GymTracker.Enums;
using GymTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.Maintenance.Routine
{
  public class EditModel(GymTrackerDbContext context, UserManager<ApplicationUser> userManager) : PageModel
  {
    [BindProperty]
    public WorkoutDay? Day { get; set; } = null!;

    // Ejercicios planificados
    public List<WorkoutDayExercise> StrengthExercisesInDay { get; set; } = [];
    public List<WorkoutDayExercise> CardioExercisesInDay { get; set; } = [];

    // Todos los ejercicios disponibles
    public List<Exercise> AllExercises { get; set; } = [];

    // Form
    [BindProperty] public int ExerciseId { get; set; }
    [BindProperty] public int PlannedSets { get; set; } = 3;
    [BindProperty] public int? PlannedReps { get; set; }
    [BindProperty] public int? PlannedDuration { get; set; }

    // ===========================
    // GET
    // ===========================
    public async Task<IActionResult> OnGetAsync(int id)
    {
      var userId = userManager.GetUserId(User)!;

      Day = await context.WorkoutDays
        .Where(wd => wd.UserId == userId)
          .Include(d => d.Exercises)
              .ThenInclude(e => e.Exercise)
          .FirstOrDefaultAsync(d => d.Id == id);

      if (Day == null)
        return NotFound();

      StrengthExercisesInDay = Day.Exercises
          .Where(e => e.Exercise.Type == ExerciseType.Strength)
          .ToList();

      CardioExercisesInDay = Day.Exercises
          .Where(e => e.Exercise.Type == ExerciseType.Cardio)
          .ToList();

      AllExercises = await context.Exercises
          .OrderBy(e => e.Name)
          .ToListAsync();

      return Page();
    }

    // ===========================
    // ADD EXERCISE (ÚNICO)
    // ===========================
    public async Task<IActionResult> OnPostAddExerciseAsync(int id)
    {
      var exercise = await context.Exercises.FindAsync(ExerciseId);
      if (exercise == null)
        return RedirectToPage(new { id });

      var exists = await context.Set<WorkoutDayExercise>()
          .AnyAsync(x => x.WorkoutDayId == id && x.ExerciseId == ExerciseId);

      if (!exists)
      {
        var entry = new WorkoutDayExercise
        {
          WorkoutDayId = id,
          ExerciseId = ExerciseId
        };

        if (exercise.Type == ExerciseType.Strength)
        {
          entry.PlannedSets = PlannedSets;
          entry.PlannedReps = PlannedReps;
          entry.PlannedDurationSeconds = PlannedDuration;
        }
        else
        {
          entry.PlannedDurationSeconds = PlannedDuration;
          entry.PlannedSets = 1;
        }

        context.Add(entry);
        await context.SaveChangesAsync();
      }

      return RedirectToPage(new { id });
    }

    // ===========================
    // REMOVE
    // ===========================
    public async Task<IActionResult> OnPostRemoveExerciseAsync(int id, int exId)
    {
      var item = await context.Set<WorkoutDayExercise>()
          .FirstOrDefaultAsync(e => e.Id == exId && e.WorkoutDayId == id);

      if (item != null)
      {
        context.Remove(item);
        await context.SaveChangesAsync();
      }

      return RedirectToPage(new { id });
    }
  }
}
