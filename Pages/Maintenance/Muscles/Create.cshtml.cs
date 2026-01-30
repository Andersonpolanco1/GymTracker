using GymTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
    if (!ModelState.IsValid)
      return Page();

    context.Muscles.Add(Muscle);
    await context.SaveChangesAsync();

    return RedirectToPage("Index");
  }
}
