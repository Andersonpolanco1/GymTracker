using GymTracker.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.Maintenance.Exercises;

public class IndexModel(GymTrackerDbContext context) : PageModel
{
  private readonly GymTrackerDbContext _context = context;

  public List<Exercise> Exercises { get; set; } = [];

  public async Task OnGetAsync()
  {
    Exercises = await _context.Exercises
        .Include(e => e.Muscle)
        .OrderBy(e => e.Type)
        .ThenBy(e => e.Name)
        .ToListAsync();
  }
}
