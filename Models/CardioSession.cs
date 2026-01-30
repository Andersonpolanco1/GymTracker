namespace GymTracker.Models
{
  public class CardioSession
  {
    public int Id { get; set; }

    public int WorkoutSessionId { get; set; }
    public WorkoutSession WorkoutSession { get; set; } = null!;

    public int ExerciseId { get; set; } // Caminadora, bici
    public Exercise Exercise { get; set; } = null!;

    public int DurationMinutes { get; set; }
    public decimal? DistanceKm { get; set; }
    public int? Calories { get; set; }
    public int? AvgHeartRate { get; set; }
  }

}
