using GymTracker.Models;
using GymTracker.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.Maintenance.Cardio
{
  public class IndexModel : PageModel
  {
    private readonly GymTrackerDbContext _context;

    public IndexModel(GymTrackerDbContext context)
    {
      _context = context;
    }

    public List<Exercise> CardioExercises { get; set; } = new();

    public async Task OnGetAsync()
    {
      CardioExercises = await _context.Exercises
          .Where(e => e.Type == ExerciseType.Cardio)
          .OrderBy(e => e.Name)
          .ToListAsync();
    }
  }
}
