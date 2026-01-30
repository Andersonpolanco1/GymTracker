using GymTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.Maintenance.Muscles
{
  public class MusclesModel(GymTrackerDbContext context) : PageModel
  {
    public List<Muscle> Muscles { get; set; } = [];

    public async Task OnGetAsync()
    {
      Muscles = await context.Muscles
          .OrderBy(x => x.Name)
          .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
      var muscle = await context.Muscles.FindAsync(id);
      if (muscle != null)
      {
        context.Muscles.Remove(muscle);
        await context.SaveChangesAsync();
      }

      return RedirectToPage();
    }
  }
}
