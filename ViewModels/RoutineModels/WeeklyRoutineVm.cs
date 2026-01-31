namespace GymTracker.ViewModels.RoutineModels
{
  public class WeeklyRoutineVm
  {
    public DayOfWeek DayOfWeek { get; set; }
    public string DayName { get; set; } = string.Empty;

    public List<MuscleRoutineVm> Muscles { get; set; } = [];
    public List<RoutineExerciseVm> Cardio { get; set; } = [];
  }

}
