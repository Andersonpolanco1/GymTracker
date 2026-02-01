using GymTracker.Enums;
using GymTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GymTracker.Pages.WorkoutSessions
{
  public class CreateModel(
      GymTrackerDbContext context,
      UserManager<ApplicationUser> userManager) : PageModel
  {

    private string UserId => userManager.GetUserId(User)!;

    public WorkoutSession? Session { get; set; }

    // Ejercicios
    public List<Exercise> StrengthRoutineExercises { get; set; } = [];
    public List<Exercise> StrengthOtherExercises { get; set; } = [];
    public List<Exercise> CardioRoutineExercises { get; set; } = [];
    public List<Exercise> CardioOtherExercises { get; set; } = [];

    // Series
    public List<ExerciseSet> Sets { get; set; } = [];
    public List<CardioSession> CardioSessions { get; set; } = [];

    // Inputs fuerza
    [BindProperty] public int SelectedStrengthId { get; set; }
    [BindProperty] public int? Reps { get; set; }
    [BindProperty] public decimal? Weight { get; set; }

    // Inputs cardio
    [BindProperty] public int SelectedCardioId { get; set; }
    [BindProperty] public int? DurationMinutes { get; set; }
    [BindProperty] public decimal? DistanceKm { get; set; }
    [BindProperty] public int? Calories { get; set; }
    [BindProperty] public int? AvgHeartRate { get; set; }

    [BindProperty] public string ActiveTab { get; set; } = "strength";

    // Fecha seleccionada
    [BindProperty] public DateTime? SelectedDate { get; set; }

    // ================= GET =================
    public async Task<IActionResult> OnGetAsync(DateTime? date)
    {
      SelectedDate = date ?? Utils.Utilities.NowRD();
      await LoadExercisesAsync(SelectedDate.Value);
      return Page();
    }

    // ================= CARGA GENERAL =================
    private async Task LoadExercisesAsync(DateTime date)
    {
      var dayOfWeek = date.DayOfWeek;

      var workoutDay = await context.WorkoutDays
          .Include(d => d.Exercises)
              .ThenInclude(de => de.Exercise)
          .FirstOrDefaultAsync(d => d.DayOfWeek == dayOfWeek);

      if (workoutDay == null) return;

      var routineIds = workoutDay.Exercises.Select(e => e.ExerciseId).ToList();

      StrengthRoutineExercises = workoutDay.Exercises
          .Where(e => e.Exercise.Type == ExerciseType.Strength)
          .Select(e => e.Exercise)
          .OrderBy(e => e.Name)
          .ToList();

      StrengthOtherExercises = await context.Exercises
          .Where(e => e.Type == ExerciseType.Strength && !routineIds.Contains(e.Id))
          .OrderBy(e => e.Name)
          .ToListAsync();

      CardioRoutineExercises = workoutDay.Exercises
          .Where(e => e.Exercise.Type == ExerciseType.Cardio)
          .Select(e => e.Exercise)
          .OrderBy(e => e.Name)
          .ToList();

      CardioOtherExercises = await context.Exercises
          .Where(e => e.Type == ExerciseType.Cardio && !routineIds.Contains(e.Id))
          .OrderBy(e => e.Name)
          .ToListAsync();

      //  Cargar sesión SOLO del usuario
      Session = await context.WorkoutSessions
          .Include(s => s.Sets)
              .ThenInclude(es => es.Exercise)
          .Include(s => s.CardioSessions)
              .ThenInclude(cs => cs.Exercise)
          .FirstOrDefaultAsync(s =>
              s.Date == date &&
              s.UserId == UserId);

      Sets = Session?.Sets.ToList() ?? [];
      CardioSessions = Session?.CardioSessions.ToList() ?? [];
    }

    // ================= FUERZA =================
    public async Task<IActionResult> OnPostAddStrengthAsync()
    {
      if (SelectedStrengthId == 0)
        ModelState.AddModelError(nameof(SelectedStrengthId), "Debes seleccionar un ejercicio");

      if (Reps is null || Reps <= 0)
        ModelState.AddModelError(nameof(Reps), "Debes indicar las repeticiones");

      if (Weight is null || Weight <= 0)
        ModelState.AddModelError(nameof(Weight), "Debes indicar el peso");

      if (!ModelState.IsValid)
        return await OnGetAsync(SelectedDate);

      var session = await GetOrCreateSessionAsync(SelectedDate ?? DateTime.Today);

      int lastSetNumber = await context.ExerciseSets
          .Where(s => s.WorkoutSessionId == session.Id && s.ExerciseId == SelectedStrengthId)
          .Select(s => (int?)s.SetNumber)
          .MaxAsync() ?? 0;

      context.ExerciseSets.Add(new ExerciseSet
      {
        WorkoutSessionId = session.Id,
        ExerciseId = SelectedStrengthId,
        SetNumber = lastSetNumber + 1,
        Reps = Reps!.Value,
        Weight = Weight!.Value
      });

      await context.SaveChangesAsync();

      return RedirectToPage(new { date = SelectedDate?.ToString("yyyy-MM-dd") });
    }

    // ================= CARDIO =================
    public async Task<IActionResult> OnPostAddCardioAsync()
    {
      if (SelectedCardioId == 0)
        ModelState.AddModelError(nameof(SelectedCardioId), "Debes seleccionar un ejercicio");

      if (DurationMinutes is null || DurationMinutes <= 0)
        ModelState.AddModelError(nameof(DurationMinutes), "Debes indicar la duración");

      if (!ModelState.IsValid)
        return await OnGetAsync(SelectedDate);

      var session = await GetOrCreateSessionAsync(SelectedDate ?? DateTime.Today);

      context.CardioSessions.Add(new CardioSession
      {
        WorkoutSessionId = session.Id,
        ExerciseId = SelectedCardioId,
        DurationMinutes = DurationMinutes!.Value,
        DistanceKm = DistanceKm,
        Calories = Calories,
        AvgHeartRate = AvgHeartRate
      });

      await context.SaveChangesAsync();

      return RedirectToPage(new { date = SelectedDate?.ToString("yyyy-MM-dd") });
    }

    // ================= REMOVE =================
    public async Task<IActionResult> OnPostRemoveStrengthAsync(int id)
    {
      var set = await context.ExerciseSets.FindAsync(id);
      if (set != null)
      {
        context.ExerciseSets.Remove(set);
        await context.SaveChangesAsync();
      }

      return RedirectToPage(new { date = SelectedDate?.ToString("yyyy-MM-dd") });
    }

    public async Task<IActionResult> OnPostRemoveCardioAsync(int id)
    {
      var cardio = await context.CardioSessions.FindAsync(id);
      if (cardio != null)
      {
        context.CardioSessions.Remove(cardio);
        await context.SaveChangesAsync();
      }

      return RedirectToPage(new { date = SelectedDate?.ToString("yyyy-MM-dd") });
    }

    // ================= SESIÓN =================
    private async Task<WorkoutSession> GetOrCreateSessionAsync(DateTime date)
    {
      var workoutDay = await context.WorkoutDays
          .FirstAsync(d => d.DayOfWeek == date.DayOfWeek);

      var session = await context.WorkoutSessions
          .FirstOrDefaultAsync(s =>
              s.Date == date &&
              s.UserId == UserId);

      if (session == null)
      {
        session = new WorkoutSession
        {
          Date = date,
          WorkoutDayId = workoutDay.Id,
          UserId = UserId
        };

        context.WorkoutSessions.Add(session);
        await context.SaveChangesAsync();
      }

      return session;
    }
  }
}
