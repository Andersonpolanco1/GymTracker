using GymTracker.Pages.Maintenance.WorkoutDays;

namespace GymTracker.ViewModels.MaintenanceModels
{
  public class WorkoutDaySummaryVm
  {
    public int Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }

    public List<MuscleGroupVm> MuscleGroups { get; set; } = [];

    public List<CardioItemVm> CardioExercises { get; set; } = [];
  }
}
