using GymTracker.Models;
using GymTracker.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;

namespace GymTracker.Pages.Maintenance.Exercises;

public class EditModel(GymTrackerDbContext context, IWebHostEnvironment environment) : PageModel
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
              .AnyAsync(e => e.Name == Exercise.Name && e.Id != Exercise.Id);

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

    if (Exercise.ImageFile != null)
    {
      // Opcional: Borrar la imagen anterior si existe para ahorrar espacio
      DeleteOldImage(Exercise.ImagePath);

      // Subir la nueva
      Exercise.ImagePath = await ProcessUploadedFile(Exercise.ImageFile, Exercise.Id);
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

  private async Task<string> ProcessUploadedFile(IFormFile file, int exerciseId)
  {
    // Carpeta destino
    string uploadsFolder = Path.Combine(environment.WebRootPath, "uploads", "exercises");

    if (!Directory.Exists(uploadsFolder))
      Directory.CreateDirectory(uploadsFolder);

    string safeFileName = Path.GetFileName(file.FileName).Replace(" ", "_");

    // Formato: {ID}_{NombreOriginal} -> Ejemplo: 5_sentadilla.gif
    string uniqueFileName = $"{exerciseId}_{safeFileName}";
    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

    using (var fileStream = new FileStream(filePath, FileMode.Create))
    {
      await file.CopyToAsync(fileStream);
    }

    return $"/uploads/exercises/{uniqueFileName}";
  }

  private void DeleteOldImage(string? relativePath)
  {
    if (string.IsNullOrEmpty(relativePath)) return;

    string fullPath = Path.Combine(environment.WebRootPath, relativePath.TrimStart('/'));

    if (System.IO.File.Exists(fullPath))
    {
      System.IO.File.Delete(fullPath);
    }
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
