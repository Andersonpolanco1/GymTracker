namespace GymTracker.Models
{
  public class WorkoutDayMuscle
  {
    public int WorkoutDayId { get; set; }
    public WorkoutDay WorkoutDay { get; set; } = null!;

    public int MuscleId { get; set; }
    public Muscle Muscle { get; set; } = null!;
  }

}
