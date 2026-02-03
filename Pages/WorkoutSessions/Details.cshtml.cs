using GymTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.WorkoutSessions;

public class DetailsModel(GymTrackerDbContext ctx) : PageModel
{
  public WorkoutSession? Session { get; set; } = null!;
  public List<PerformedExerciseAccordionItem> AccordionItems { get; set; } = [];

  public async Task<IActionResult> OnGetAsync(int id)
  {
    Session = await ctx.WorkoutSessions
      .Include(s => s.PerformedExercises)
        .ThenInclude(p => p.Exercise)
      .FirstOrDefaultAsync(s => s.Id == id);

    if (Session == null)
      return NotFound();

    BuildAccordionItems();

    return Page();
  }

  private void BuildAccordionItems()
  {
    AccordionItems = Session.PerformedExercises
      .GroupBy(p => p.ExerciseId)
      .Select(group =>
      {
        var first = group.First();

        var item = new PerformedExerciseAccordionItem
        {
          ExerciseId = first.ExerciseId,
          ExerciseName = first.Exercise.Name,
          Type = first switch
          {
            StrengthSet => "Fuerza",
            TimedSet => "Cardio",
            _ => "Otro"
          }
        };

        foreach (var p in group)
        {
          switch (p)
          {
            case StrengthSet s:
              item.Details.Add(new PerformedExerciseDetailRow
              {
                Label = $"Serie {item.Details.Count + 1}",
                Work = $"{s.Reps} reps",
                Load = $"{s.Weight} lb",
                Extra = $"{s.Volume} lb"
              });
              break;

            case TimedSet t:
              item.Details.Add(new PerformedExerciseDetailRow
              {
                Label = "Sesión",
                Work = $"{(int)t.Duration.TotalMinutes} min",
                Load = t.DistanceKm != null ? $"{t.DistanceKm} km" : "-",
                Extra = t.Calories != null ? $"{t.Calories} kcal" : "-"
              });
              break;
          }
        }

        item.Summary = item.Type == "Fuerza"
          ? $"{item.Details.Count} series · {item.Details.Sum(d => int.Parse(d.Work.Split(' ')[0]))} reps"
          : item.Details.First().Work;

        return item;
      })
      .OrderBy(i => i.ExerciseName)
      .ToList();
  }

  public class PerformedExerciseAccordionItem
  {
    public int ExerciseId { get; set; }
    public string ExerciseName { get; set; } = "";
    public string Type { get; set; } = "";

    public string Summary { get; set; } = "";

    public List<PerformedExerciseDetailRow> Details { get; set; } = [];
  }

  public class PerformedExerciseDetailRow
  {
    public string Label { get; set; } = "";
    public string Work { get; set; } = "";
    public string Load { get; set; } = "";
    public string Extra { get; set; } = "";
  }

}
