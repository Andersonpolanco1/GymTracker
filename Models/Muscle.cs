namespace GymTracker.Models
{
  public class Muscle
  {
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    // Navegación
    public ICollection<Exercise> Exercises { get; set; } = [];
  }

}
