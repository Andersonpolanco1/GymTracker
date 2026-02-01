using System.ComponentModel.DataAnnotations;

namespace GymTracker.Models
{
  public class Muscle
  {
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del músculo es obligatorio.")]
    public string Name { get; set; } = null!;

    // Navegación
    public ICollection<Exercise> Exercises { get; set; } = [];
  }

}
