namespace GymTracker.Models
{
  public class WorkoutSession
  {
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public DateTime CreatedAt { get; set; }

    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    public int WorkoutDayId { get; set; }
    public WorkoutDay WorkoutDay { get; set; } = null!;

    public ICollection<PerformedExercise> PerformedExercises { get; set; } = [];
  }
}
