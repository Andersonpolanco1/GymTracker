using GymTracker.Pages.Maintenance.WorkoutDays;

namespace GymTracker.ViewModels.MaintenanceModels
{
  public class MuscleGroupVm
  {
    public string MuscleName { get; set; } = null!;
    public List<ExerciseItemVm> Exercises { get; set; } = [];
  }
}
