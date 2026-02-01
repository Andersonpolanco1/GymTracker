using Microsoft.AspNetCore.Identity;

namespace GymTracker.Models
{
  public class ApplicationUser : IdentityUser
  {
    public string DisplayName { get; set; } = string.Empty;
    public ICollection<WorkoutDay> WorkoutDays { get; set; } = [];
    public ICollection<WorkoutSession> WorkoutSessions { get; set; } = [];

  }

}
