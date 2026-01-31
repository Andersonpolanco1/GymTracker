using GymTracker.Enums;
using GymTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.WorkoutSessions
{
  public class TodayModel(GymTrackerDbContext ctx) : PageModel
  {
    public WorkoutSession? Session { get; set; }
    public int WorkoutSessionId { get; set; }

    // Ejercicios programados hoy
    public List<Exercise> StrengthExercises { get; set; } = [];
    public List<Exercise> CardioExercises { get; set; } = [];

    // Series registradas
    public List<ExerciseSet> Sets { get; set; } = [];
    public List<CardioSession> CardioSessions { get; set; } = [];

    // BindProperties para fuerza
    [BindProperty] public int SelectedStrengthId { get; set; }
    [BindProperty] public int? Reps { get; set; }
    [BindProperty] public decimal Weight { get; set; }

    // BindProperties para cardio
    [BindProperty] public int SelectedCardioId { get; set; }
    [BindProperty] public int? DurationMinutes { get; set; }
    [BindProperty] public decimal? DistanceKm { get; set; }
    [BindProperty] public int? Calories { get; set; }
    [BindProperty] public int? AvgHeartRate { get; set; }

    // Fuerza
    public List<Exercise> StrengthRoutineExercises { get; set; } = [];
    public List<Exercise> StrengthOtherExercises { get; set; } = [];

    // Cardio
    public List<Exercise> CardioRoutineExercises { get; set; } = [];
    public List<Exercise> CardioOtherExercises { get; set; } = [];


    public async Task<IActionResult> OnGetAsync()
    {
      var today = DateTime.Today;
      var dayOfWeek = today.DayOfWeek;

      // Traer WorkoutDay del día actual con ejercicios
      var workoutDay = await ctx.WorkoutDays
          .Include(d => d.Exercises)
              .ThenInclude(de => de.Exercise)
          .FirstOrDefaultAsync(d => d.DayOfWeek == dayOfWeek);

      if (workoutDay == null) return NotFound("No hay entrenamiento programado para hoy.");

      // Obtener o crear la sesión del día
      Session = await GetOrCreateTodaySessionAsync();

      WorkoutSessionId = Session.Id;

      // IDs de ejercicios del día
      var routineExerciseIds = workoutDay.Exercises
          .Select(e => e.ExerciseId)
          .ToList();

      // ================= FUERZA =================
      StrengthRoutineExercises = workoutDay.Exercises
          .Where(e => e.Exercise.Type == ExerciseType.Strength)
          .Select(e => e.Exercise)
          .OrderBy(e => e.Name)
          .ToList();

      StrengthOtherExercises = await ctx.Exercises
          .Where(e =>
              e.Type == ExerciseType.Strength &&
              !routineExerciseIds.Contains(e.Id))
          .OrderBy(e => e.Name)
          .ToListAsync();

      // ================= CARDIO =================
      CardioRoutineExercises = workoutDay.Exercises
          .Where(e => e.Exercise.Type == ExerciseType.Cardio)
          .Select(e => e.Exercise)
          .OrderBy(e => e.Name)
          .ToList();

      CardioOtherExercises = await ctx.Exercises
          .Where(e =>
              e.Type == ExerciseType.Cardio &&
              !routineExerciseIds.Contains(e.Id))
          .OrderBy(e => e.Name)
          .ToListAsync();


      Sets = await ctx.ExerciseSets
          .Where(s => s.WorkoutSessionId == Session.Id)
          .Include(s => s.Exercise)
          .ToListAsync();

      CardioSessions = await ctx.CardioSessions
          .Where(c => c.WorkoutSessionId == Session.Id)
          .Include(c => c.Exercise)
          .ToListAsync();

      return Page();
    }

    public async Task<IActionResult> OnPostAddStrengthAsync()
    {
      if (SelectedStrengthId == 0 || Reps is null || Reps <= 0)
      {
        ModelState.AddModelError("", "Datos inválidos");
        return await OnGetAsync();
      }

      var session = await GetOrCreateTodaySessionAsync();

      // calcular siguiente número de serie
      var query = ctx.ExerciseSets
          .Where(s =>
              s.WorkoutSessionId == session.Id &&
              s.ExerciseId == SelectedStrengthId);

      int lastSetNumber = await query.AnyAsync()
          ? await query.MaxAsync(s => s.SetNumber)
          : 0;

      ctx.ExerciseSets.Add(new ExerciseSet
        {
          WorkoutSessionId = session.Id, 
          ExerciseId = SelectedStrengthId,
          SetNumber = lastSetNumber +1,
          Reps = Reps.Value,
          Weight = Weight
        });

      await ctx.SaveChangesAsync();
      return RedirectToPage();
    }


    public async Task<IActionResult> OnPostAddCardioAsync()
    {
      if (SelectedCardioId == 0 || DurationMinutes is null|| DurationMinutes <= 0)
      {
        ModelState.AddModelError("", "Datos de cardio inválidos");
        return await OnGetAsync();
      }

      var session = await GetOrCreateTodaySessionAsync();

      ctx.CardioSessions.Add(new CardioSession
      {
        WorkoutSessionId = session.Id, 
        ExerciseId = SelectedCardioId,
        DurationMinutes = DurationMinutes.Value,
        DistanceKm = DistanceKm,
        Calories = Calories,
        AvgHeartRate = AvgHeartRate
      });

      await ctx.SaveChangesAsync();
      return RedirectToPage();
    }


    public async Task<IActionResult> OnPostRemoveStrengthAsync(int id)
    {
      var set = await ctx.ExerciseSets.FindAsync(id);
      if (set != null)
      {
        ctx.ExerciseSets.Remove(set);
        await ctx.SaveChangesAsync();
      }
      return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveCardioAsync(int id)
    {
      var cardio = await ctx.CardioSessions.FindAsync(id);
      if (cardio != null)
      {
        ctx.CardioSessions.Remove(cardio);
        await ctx.SaveChangesAsync();
      }
      return RedirectToPage();
    }

    private async Task<WorkoutSession> GetOrCreateTodaySessionAsync()
    {
      var today = DateTime.Today;
      var dayOfWeek = today.DayOfWeek;

      var workoutDay = await ctx.WorkoutDays
          .FirstAsync(d => d.DayOfWeek == dayOfWeek);

      var session = await ctx.WorkoutSessions
          .FirstOrDefaultAsync(s => s.Date == today);

      if (session == null)
      {
        session = new WorkoutSession
        {
          Date = today,
          WorkoutDayId = workoutDay.Id
        };

        ctx.WorkoutSessions.Add(session);
        await ctx.SaveChangesAsync(); 
      }

      return session;
    }

  }
}
