using GymTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.Maintenance.Muscles;

public class CreateModel(GymTrackerDbContext context) : PageModel
{
  [BindProperty]
  public Muscle Muscle { get; set; } = new();

  public void OnGet()
  {

  }

  public async Task<IActionResult> OnPostAsync()
  {
    bool exists = await context.Muscles
      .AnyAsync(e => e.Name == Muscle.Name);

    if (exists)
    {
      ModelState.AddModelError("Muscle.Name",
          "Ya existe un músculo con este nombre.");
    }

    if (!ModelState.IsValid)
      return Page();

    context.Muscles.Add(Muscle);
    await context.SaveChangesAsync();

    return RedirectToPage("Index");
  }
}
