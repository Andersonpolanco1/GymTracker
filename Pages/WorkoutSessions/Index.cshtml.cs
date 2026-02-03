using GymTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.WorkoutSessions
{
  public class IndexModel : PageModel
  {
    private readonly GymTrackerDbContext ctx;
    public IndexModel(GymTrackerDbContext context) => ctx = context;

    public List<WorkoutSessionRow> Sessions { get; set; } = new();

    // Filtros
    [BindProperty(SupportsGet = true)]
    public int? SelectedYear { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? SelectedMonth { get; set; }

    // Dropdowns
    public List<int> Years { get; set; } = new();
    public List<SelectListItem> Months { get; set; } = new();

    // Diccionario de nombres de meses en español
    private readonly Dictionary<int, string> MonthNames = new()
        {
            { 1, "Enero" }, { 2, "Febrero" }, { 3, "Marzo" }, { 4, "Abril" },
            { 5, "Mayo" }, { 6, "Junio" }, { 7, "Julio" }, { 8, "Agosto" },
            { 9, "Septiembre" }, { 10, "Octubre" }, { 11, "Noviembre" }, { 12, "Diciembre" }
        };

    public async Task OnGetAsync()
    {
      // Obtener años con sesiones
      Years = await ctx.WorkoutSessions
          .Select(s => s.Date.Year)
          .Distinct()
          .OrderByDescending(y => y)
          .ToListAsync();

      // Query base
      var query = ctx.WorkoutSessions
          .Include(s => s.WorkoutDay)
          .Include(s => s.PerformedExercises)
          .AsQueryable();

      // Filtrar por año y mes si hay
      if (SelectedYear.HasValue)
        query = query.Where(s => s.Date.Year == SelectedYear.Value);

      if (SelectedMonth.HasValue)
        query = query.Where(s => s.Date.Month == SelectedMonth.Value);

      var sessions = await query
          .OrderByDescending(s => s.Date)
          .ToListAsync();

      // Mapear sesiones
      Sessions = sessions.Select(s =>
      {
        var strength = s.PerformedExercises.OfType<StrengthSet>();
        var cardio = s.PerformedExercises.OfType<TimedSet>();

        return new WorkoutSessionRow
        {
          Id = s.Id,
          Date = s.Date,
          DayName = Utils.Utilities.GetDayNameInSpanishCapitalized(s.WorkoutDay.DayOfWeek),
          StrengthSets = strength.Count(),
          TotalVolume = strength.Sum(x => x.Reps!.Value * x.Weight!.Value),
          CardioMinutes = (int)cardio.Sum(c => c.Duration.TotalMinutes)
        };
      }).ToList();

      // Meses disponibles según sesiones filtradas por año
      var availableMonths = sessions
          .Select(s => s.Date.Month)
          .Distinct()
          .OrderBy(m => m)
          .ToList();

      Months = availableMonths
          .Select(m => new SelectListItem
          {
            Value = m.ToString(),
            Text = MonthNames[m]
          })
          .ToList();
    }
  }

  public class WorkoutSessionRow
  {
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string DayName { get; set; } = "";
    public int StrengthSets { get; set; }
    public decimal TotalVolume { get; set; }
    public int CardioMinutes { get; set; }
  }
}
