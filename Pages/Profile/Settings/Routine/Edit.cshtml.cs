using GymTracker.Enums;
using GymTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.Maintenance.Routine
{
  public class EditModel : PageModel
  {
    private readonly GymTrackerDbContext _context;

    public EditModel(GymTrackerDbContext context)
    {
      _context = context;
    }

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
      Day = await _context.WorkoutDays
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

      AllExercises = await _context.Exercises
          .OrderBy(e => e.Name)
          .ToListAsync();

      return Page();
    }

    // ===========================
    // ADD EXERCISE (ÚNICO)
    // ===========================
    public async Task<IActionResult> OnPostAddExerciseAsync(int id)
    {
      var exercise = await _context.Exercises.FindAsync(ExerciseId);
      if (exercise == null)
        return RedirectToPage(new { id });

      var exists = await _context.Set<WorkoutDayExercise>()
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

        _context.Add(entry);
        await _context.SaveChangesAsync();
      }

      return RedirectToPage(new { id });
    }

    // ===========================
    // REMOVE
    // ===========================
    public async Task<IActionResult> OnPostRemoveExerciseAsync(int id, int exId)
    {
      var item = await _context.Set<WorkoutDayExercise>()
          .FirstOrDefaultAsync(e => e.Id == exId && e.WorkoutDayId == id);

      if (item != null)
      {
        _context.Remove(item);
        await _context.SaveChangesAsync();
      }

      return RedirectToPage(new { id });
    }
  }
}
