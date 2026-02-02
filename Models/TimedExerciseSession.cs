namespace GymTracker.Models
{
  public class TimedExerciseSession : PerformedExercise
  {
    public TimeSpan Duration { get; set; }
    public decimal? DistanceKm { get; set; }
    public int? Calories { get; set; }
    public int? AvgHeartRate { get; set; }
  }


}
