using GymTracker.Enums;
using System.ComponentModel.DataAnnotations;

namespace GymTracker.Models
{
  public class Exercise
  {
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del ejercicio es obligatorio.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Debe seleccionar un tipo de ejercicio.")]
    public ExerciseType Type { get; set; }

    public int? MuscleId { get; set; }
    public Muscle? Muscle { get; set; }

    public string? Notes { get; set; }

  }

}
