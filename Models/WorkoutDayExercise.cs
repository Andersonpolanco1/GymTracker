namespace GymTracker.Models
{
  public class WorkoutDayExercise
  {
    public int Id { get; set; }

    public int WorkoutDayId { get; set; }
    public WorkoutDay WorkoutDay { get; set; } = null!;

    public int ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    public int PlannedSets { get; set; }
    public int? PlannedReps { get; set; }
    public int? PlannedDurationSeconds { get; set; }
  }

}
