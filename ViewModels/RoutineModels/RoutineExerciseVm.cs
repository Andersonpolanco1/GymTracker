using GymTracker.Enums;

namespace GymTracker.ViewModels.RoutineModels
{
  public class RoutineExerciseVm
  {
    public string Name { get; set; } = string.Empty;
    public int Sets { get; set; }
    public int? Reps { get; set; }
    public int? DurationSeconds { get; set; }
    public ExerciseType Type { get; set; }
  }
}
