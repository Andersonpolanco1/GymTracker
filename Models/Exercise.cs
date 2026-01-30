using GymTracker.Enums;

namespace GymTracker.Models
{
  public class Exercise
  {
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ExerciseType Type { get; set; }

    public int? MuscleId { get; set; }
    public Muscle? Muscle { get; set; }
  }

}
