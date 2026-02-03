using GymTracker.Enums;
using GymTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.Maintenance.Exercises;

public class CreateModel(GymTrackerDbContext context) : PageModel
{
  private readonly GymTrackerDbContext _context = context;

  [BindProperty]
  public Exercise Exercise { get; set; } = new();

  public SelectList Muscles { get; set; } = default!;

  public async Task OnGetAsync()
  {
    await LoadMusclesAsync();
  }

  public async Task<IActionResult> OnPostAsync()
  {
    if (Exercise.Type == ExerciseType.Cardio)
    {
      Exercise.MuscleId = null;
    }

    if (Exercise.Type == ExerciseType.Strength && Exercise.MuscleId == null)
    {
      ModelState.AddModelError("Exercise.MuscleId",
          "Debe seleccionar un músculo para ejercicios de fuerza.");
    }

    bool exists = await _context.Exercises
        .AnyAsync(e => e.Name == Exercise.Name);
    if (exists)
    {
      ModelState.AddModelError("Exercise.Name",
          "Ya existe un ejercicio con este nombre.");
    }

    if (!ModelState.IsValid)
    {
      await LoadMusclesAsync();
      return Page();
    }

    _context.Exercises.Add(Exercise);
    await _context.SaveChangesAsync();
    return RedirectToPage("Index");
  }



  private async Task LoadMusclesAsync()
  {
    Muscles = new SelectList(
      await _context.Muscles
        .OrderBy(m => m.Name)
        .ToListAsync(),
      "Id",
      "Name");
  }
}
