namespace GymTracker.Models
{
  public class WorkoutDay
  {
    public int Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }

    public string UserId { get; set; } = null!;

    // Navegación
    public ApplicationUser User { get; set; } = null!;
    public ICollection<WorkoutDayExercise> Exercises { get; set; } = [];
    public ICollection<WorkoutSession> WorkoutSessions { get; set; } = [];
  }

}
