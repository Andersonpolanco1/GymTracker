namespace GymTracker.Models
{
  public class WorkoutSession
  {
    public int Id { get; set; }
    public DateTime Date { get; set; }

    public int WorkoutDayId { get; set; }
    public WorkoutDay WorkoutDay { get; set; } = null!;

    public ICollection<ExerciseSet> Sets { get; set; } = [];
    public ICollection<CardioSession> CardioSessions { get; set; } = [];
  }

}
