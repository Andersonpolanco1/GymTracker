namespace GymTracker.Models
{
  public class StrengthSet : PerformedExercise
  {
    public int? Reps { get; set; }
    public decimal? Weight { get; set; }
    public int? RIR { get; set; }
    public int? RestSeconds { get; set; }
    public TimeSpan? Duration { get; set; }


    public decimal? Volume =>
      Reps.HasValue && Weight.HasValue
        ? Reps * Weight
        : null;
  }

}
