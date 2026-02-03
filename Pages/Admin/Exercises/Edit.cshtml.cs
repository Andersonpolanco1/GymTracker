using GymTracker.Models;
using GymTracker.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.Maintenance.Exercises;

public class EditModel(GymTrackerDbContext context) : PageModel
{
  private readonly GymTrackerDbContext _context = context;

  [BindProperty]
  public Exercise? Exercise { get; set; } 

  public SelectList Muscles { get; set; } = default!;
  public SelectList ExerciseTypes { get; set; } = default!;

  public async Task<IActionResult> OnGetAsync(int id)
  {
    Exercise = await _context.Exercises
        .Include(e => e.Muscle)
        .FirstOrDefaultAsync(e => e.Id == id);

    if (Exercise == null)
      return NotFound();

    LoadSelectLists();
    return Page();
  }

  public async Task<IActionResult> OnPostAddAsync()
  {
    if (Exercise is not null)
    {
      bool exists = await _context.Exercises
        .AnyAsync(e => e.Name == Exercise.Name);
            if (exists)
            {
              ModelState.AddModelError("Exercise.Name",
                  "Ya existe un ejercicio con este nombre.");
            }

      if (Exercise.Type == ExerciseType.Strength && Exercise.MuscleId == null)
      {
        ModelState.AddModelError("Exercise.MuscleId",
            "Debe seleccionar un músculo para ejercicios de fuerza.");
      }
    }


    if (!ModelState.IsValid)
    {
      LoadSelectLists();
      return Page();
    }

    if (Exercise!.Type == ExerciseType.Cardio)
    {
      Exercise.MuscleId = null;
    }

    _context.Attach(Exercise).State = EntityState.Modified;
    await _context.SaveChangesAsync();

    return RedirectToPage("Index");
  }

  public async Task<IActionResult> OnPostDeleteAsync(int id)
  {
    var exercise = await _context.Exercises.FindAsync(id);
    if (exercise == null)
      return NotFound();

    bool isUsed = await _context.Set<WorkoutDayExercise>()
        .AnyAsync(w => w.ExerciseId == id);

    if (isUsed)
    {
      TempData["ErrorMessage"] = "No se puede eliminar este ejercicio porque ya está asignado a rutinas.";
      return RedirectToPage(new { id });
    }

    _context.Exercises.Remove(exercise);
    await _context.SaveChangesAsync();

    TempData["SuccessMessage"] = "Ejercicio eliminado correctamente.";
    return RedirectToPage("Index");
  }


  private void LoadSelectLists()
  {
    Muscles = new SelectList(
        _context.Muscles
            .OrderBy(m => m.Name),
        "Id",
        "Name"
    );

    ExerciseTypes = new SelectList(
        Enum.GetValues<ExerciseType>()
    );
  }
}
