namespace GymTracker.Models
{
  public class WorkoutDay
  {
    public int Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }

    // Navegación
    public ICollection<WorkoutDayExercise> Exercises { get; set; } = [];
  }

}
