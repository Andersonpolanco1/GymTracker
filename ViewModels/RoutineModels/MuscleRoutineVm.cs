namespace GymTracker.ViewModels.RoutineModels
{
  public class MuscleRoutineVm
  {
    public string MuscleName { get; set; } = string.Empty;
    public List<RoutineExerciseVm> Exercises { get; set; } = [];
  }

}
