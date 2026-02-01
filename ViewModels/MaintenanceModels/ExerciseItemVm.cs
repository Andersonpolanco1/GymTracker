namespace GymTracker.ViewModels.MaintenanceModels
{
  public class ExerciseItemVm
  {
    public string Name { get; set; } = null!;
    public int Sets { get; set; }
    public int? Reps { get; set; }
    public int? DurationsSeconds { get; set; }
  }
}
