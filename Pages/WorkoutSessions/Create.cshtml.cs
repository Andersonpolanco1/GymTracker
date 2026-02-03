using GymTracker.Enums;
using GymTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;

namespace GymTracker.Pages.WorkoutSessions
{
  public class CreateModel(
      GymTrackerDbContext context,
      UserManager<ApplicationUser> userManager) : PageModel
  {
    private string UserId => userManager.GetUserId(User)!;

    public DateOnly SelectedDate { get; set; }
    public bool IsToday { get; set; }
    public string DayName { get; set; } = "";

    public List<int> WorkoutDays { get; set; } = [];
    public List<ExerciseAccordionItemVm> RoutineExercises { get; set; } = [];
    public List<ExerciseAccordionItemVm> ExtraExercisesPerformed { get; set; } = [];
    public List<Exercise> ExtraExercises { get; set; } = [];


    // ============================
    // GET
    // ============================
    public async Task<IActionResult> OnGetAsync(DateTime? date)
    {
      var selectedDateTime = date ?? Utils.Utilities.TodayRD();
      var dateOnly = DateOnly.FromDateTime(selectedDateTime);

      await LoadPageAsync(dateOnly);

      return Page();
    }


    private async Task LoadPageAsync(DateOnly date)
    {
      InitializeDateContext(date);

      await LoadActiveWorkoutDayIdsAsync();

      if (!IsSelectedDayEnabled())
      {
        AddDisabledDayError();
        return;
      }

      var workoutDay = await LoadSelectedWorkoutDayExercisesAsync();
      var performedExercises = await GetPerformedExercisesAsync();

      LoadRoutineExercises(workoutDay, performedExercises);
      await LoadExtraExercisesAsync(workoutDay, performedExercises);
    }

    private void InitializeDateContext(DateOnly date)
    {
      SelectedDate = date;
      IsToday = SelectedDate == DateOnly.FromDateTime(Utils.Utilities.TodayRD());
      DayName = Utils.Utilities.GetDayNameInSpanish(SelectedDate.DayOfWeek);
    }

    private bool IsSelectedDayEnabled()
    {
      return WorkoutDays.Contains((int)SelectedDate.DayOfWeek);
    }

    private void AddDisabledDayError()
    {
      ModelState.AddModelError(
          "SelectedDate",
          $"El día {DayName} no está habilitado."
      );
    }
    private async Task<WorkoutDay> LoadSelectedWorkoutDayExercisesAsync()
    {
      return await context.WorkoutDays
        .Include(wd => wd.Exercises)
          .ThenInclude(wde => wde.Exercise)
        .FirstAsync(wd =>
          wd.DayOfWeek == SelectedDate.DayOfWeek &&
          wd.UserId == UserId);
    }

    private async Task<List<PerformedExercise>> GetPerformedExercisesAsync()
    {
      var session = await GetSessionAsync(SelectedDate);

      if (session == null)
        return [];

      return await context.PerformedExercises
        .Include(p => p.Exercise)
        .Where(p => p.WorkoutSessionId == session.Id)
        .ToListAsync();
    }

    private void LoadRoutineExercises(
      WorkoutDay workoutDay,
      List<PerformedExercise> performed)
    {
      RoutineExercises = [.. workoutDay.Exercises
      .Select(wde => BuildAccordionItem(wde.Exercise, performed))
      .OrderBy(x => x.ExerciseName)];
    }


    private async Task LoadExtraExercisesAsync(
      WorkoutDay workoutDay,
      List<PerformedExercise> performed)
    {
      var routineExerciseIds = workoutDay.Exercises
        .Select(x => x.ExerciseId)
        .ToHashSet();

      ExtraExercisesPerformed = [.. performed
        .Where(p => !routineExerciseIds.Contains(p.ExerciseId))
        .GroupBy(p => p.Exercise!)
        .Select(g => BuildAccordionItem(g.Key, performed))
        .OrderBy(x => x.ExerciseName)];

      ExtraExercises = await context.Exercises
        .Where(e => !routineExerciseIds.Contains(e.Id))
        .OrderBy(e => e.Name)
        .ToListAsync();
    }




    // ============================
    // ADD SET
    // ============================
    public async Task<IActionResult> OnPostAddSetAsync(
        int exerciseId,
        DateOnly date,
        int? reps,
        decimal? weight,
        int? durationMinutes,
        decimal? distanceKm,
        int? calories)
    {
      var session = await GetOrCreateSessionAsync(date);
      var exercise = await context.Exercises.FindAsync(exerciseId);

      if (exercise == null)
      {
        ModelState.AddModelError("", "Ejercicio no encontrado");
        await LoadPageAsync(date);
        return Page();
      }

      // ============================
      // VALIDATIONS
      // ============================
      if (exercise.Type == ExerciseType.Strength)
      {
        ValidateStrengthSet(reps, weight, durationMinutes);
      }
      else
      {
        ValidateTimedSet(durationMinutes);
      }

      if (!ModelState.IsValid)
      {
        await LoadPageAsync(date);
        return Page();
      }

      // ============================
      // SAVE
      // ============================
      if (exercise.Type == ExerciseType.Strength)
      {
        context.PerformedExercises.Add(new StrengthSet
        {
          WorkoutSessionId = session.Id,
          ExerciseId = exerciseId,
          Reps = reps!.Value,
          Weight = weight ?? 0,
          Duration = durationMinutes.HasValue
                ? TimeSpan.FromMinutes(durationMinutes.Value)
                : null
        });
      }
      else
      {
        context.PerformedExercises.Add(new TimedSet
        {
          WorkoutSessionId = session.Id,
          ExerciseId = exerciseId,
          Duration = TimeSpan.FromMinutes(durationMinutes!.Value),
          DistanceKm = distanceKm,
          Calories = calories
        });
      }

      await context.SaveChangesAsync();
      return RedirectToPage(new { date });
    }

    private void ValidateStrengthSet(
    int? reps,
    decimal? weight,
    int? durationMinutes)
    {
      if (!reps.HasValue || reps <= 0)
      {
        ModelState.AddModelError(
            nameof(reps),
            "Las repeticiones son obligatorias para ejercicios de fuerza"
        );
        return;
      }

      var hasWeight = weight.HasValue && weight > 0;
      var hasDuration = durationMinutes.HasValue && durationMinutes > 0;

      if (!hasWeight && !hasDuration)
      {
        ModelState.AddModelError(
            "",
            "Debes indicar peso o duración para ejercicios de fuerza"
        );

        return;
      }
    }

    private void ValidateTimedSet(int? durationMinutes)
    {
      if (!durationMinutes.HasValue || durationMinutes <= 0)
      {
        ModelState.AddModelError(
            nameof(durationMinutes),
            "La duración es obligatoria para este tipo de ejercicio"
        );
      }
    }



    // ============================
    // REMOVE SET
    // ============================
    public async Task<IActionResult> OnPostRemoveAsync(int id, DateTime date)
    {
      var set = await context.PerformedExercises.FindAsync(id);
      if (set != null)
      {
        context.PerformedExercises.Remove(set);
        await context.SaveChangesAsync();
      }

      return RedirectToPage(new { date });
    }

    // ============================
    // HELPERS
    // ============================
    private ExerciseAccordionItemVm BuildAccordionItem(
      Exercise exercise,
      List<PerformedExercise> performed)
    {
      var vm = new ExerciseAccordionItemVm
      {
        ExerciseId = exercise.Id,
        ExerciseName = exercise.Name,
        Type = exercise.Type,
        SelectedDate = SelectedDate
      };

      int order = 1;

      foreach (var p in performed.Where(x => x.ExerciseId == exercise.Id))
      {
        if (p is StrengthSet s)
        {
          vm.Sets.Add(new ExerciseSetVm
          {
            Id = s.Id,
            Order = order++,
            Reps = s.Reps,
            Weight = s.Weight,
            DurationMinutes = s.Duration.HasValue ? (int)s.Duration.Value.TotalMinutes : null
          });
        }
        else if (p is TimedSet t)
        {
          vm.Sets.Add(new ExerciseSetVm
          {
            Id = t.Id,
            Order = order++,
            DurationMinutes = (int)t.Duration.TotalMinutes,
            DistanceKm = t.DistanceKm,
            Calories = t.Calories
          });
        }
      }

      return vm;
    }

    private async Task<WorkoutSession?> GetSessionAsync(DateOnly date)
    {
      return await context.WorkoutSessions
        .FirstOrDefaultAsync(s => s.Date == date && s.UserId == UserId);
    }


    private async Task<WorkoutSession> GetOrCreateSessionAsync(DateOnly date)
    {
      var session = await GetSessionAsync(date);

      if (session != null) return session;

      var workoutDay = await context.WorkoutDays
        .FirstAsync(wd => wd.DayOfWeek == date.DayOfWeek && wd.UserId == UserId);

      session = new WorkoutSession
      {
        Date = date,
        UserId = UserId,
        WorkoutDayId = workoutDay.Id,
        CreatedAt = Utils.Utilities.TodayRD()
      };

      context.WorkoutSessions.Add(session);
      await context.SaveChangesAsync();
      return session;
    }

    private async Task LoadActiveWorkoutDayIdsAsync()
    {
      WorkoutDays = await context.WorkoutDays
        .Where(wd => wd.IsActive && wd.UserId == UserId)
        .Select(wd => (int)wd.DayOfWeek)
        .ToListAsync();
    }
  }

  // ============================
  // VIEW MODELS
  // ============================
  public class ExerciseAccordionItemVm
  {
    public int ExerciseId { get; set; }
    public string ExerciseName { get; set; } = "";
    public ExerciseType Type { get; set; }
    public List<ExerciseSetVm> Sets { get; set; } = [];
    public DateOnly SelectedDate { get; set; } 
  }

  public class ExerciseSetVm
  {
    public int Id { get; set; }
    public int Order { get; set; }

    public int? Reps { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Volume =>
        Reps.HasValue && Weight.HasValue
            ? Reps.Value * Weight.Value
            : null;


    public int? DurationMinutes { get; set; }
    public int? Calories { get; set; }
    public decimal? DistanceKm { get; set; }
  }
}
