using GymTracker.Enums;
using GymTracker.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymTracker
{

  public class GymTrackerDbContext(DbContextOptions<GymTrackerDbContext> options) : IdentityDbContext<ApplicationUser>(options)
  {
    public DbSet<Muscle> Muscles => Set<Muscle>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<WorkoutDay> WorkoutDays => Set<WorkoutDay>();
    public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();
    public DbSet<ExerciseSet> ExerciseSets => Set<ExerciseSet>();
    public DbSet<CardioSession> CardioSessions => Set<CardioSession>();
    public DbSet<WorkoutDayExercise> WorkoutDayExercises => Set<WorkoutDayExercise>();
    public DbSet<PerformedExercise> PerformedExercises => Set<PerformedExercise>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<PerformedExercise>()
        .HasDiscriminator<string>("Discriminator")
        .HasValue<StrengthSet>("Strength")
        .HasValue<TimedExerciseSession>("Timed");


      // ============================================================
      // USER -> WORKOUT DAYS (CASCADE)
      // ============================================================
      modelBuilder.Entity<WorkoutDay>()
        .HasOne(wd => wd.User)
        .WithMany(u => u.WorkoutDays)
        .HasForeignKey(wd => wd.UserId)
        .OnDelete(DeleteBehavior.Cascade);

      // ============================================================
      // WORKOUT DAY <-> EXERCISES (MANY TO MANY WITH PAYLOAD)
      // ============================================================
      modelBuilder.Entity<WorkoutDayExercise>()
        .HasOne(x => x.WorkoutDay)
        .WithMany(d => d.Exercises)
        .HasForeignKey(x => x.WorkoutDayId)
        .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<WorkoutDayExercise>()
        .HasOne(x => x.Exercise)
        .WithMany()
        .HasForeignKey(x => x.ExerciseId)
        .OnDelete(DeleteBehavior.Restrict);

      // ============================================================
      // USER -> WORKOUT SESSIONS (CASCADE)
      // ============================================================
      modelBuilder.Entity<WorkoutSession>()
        .HasOne(ws => ws.User)
        .WithMany(u => u.WorkoutSessions)
        .HasForeignKey(ws => ws.UserId)
        .OnDelete(DeleteBehavior.Cascade);

      // ============================================================
      // WORKOUT DAY -> WORKOUT SESSIONS (NO CASCADE ❗)
      // evita multiple cascade paths
      // ============================================================
      modelBuilder.Entity<WorkoutSession>()
        .HasOne(ws => ws.WorkoutDay)
        .WithMany(wd => wd.WorkoutSessions)
        .HasForeignKey(ws => ws.WorkoutDayId)
        .OnDelete(DeleteBehavior.Restrict);

      // ============================================================
      // DECIMAL CONFIGURATION (fix warnings)
      // ============================================================
      modelBuilder.Entity<TimedExerciseSession>()
        .Property(c => c.DistanceKm)
        .HasPrecision(6, 2);

      modelBuilder.Entity<StrengthSet>()
        .Property(e => e.Weight)
        .HasPrecision(6, 2);



      modelBuilder.Entity<Muscle>().HasData(
          new Muscle { Id = 1, Name = "Espalda" },
          new Muscle { Id = 2, Name = "Biceps" },
          new Muscle { Id = 3, Name = "Piernas" },
          new Muscle { Id = 4, Name = "Hombros" },
          new Muscle { Id = 5, Name = "Pecho" },
          new Muscle { Id = 6, Name = "Triceps" },
          new Muscle { Id = 7, Name = "Abdomen" }
      );

      modelBuilder.Entity<Exercise>().HasData(
          // Espalda
          new Exercise { Id = 1, Name = "Peso muerto (espalda baja)", MuscleId = 1, Type = ExerciseType.Strength },
          new Exercise { Id = 2, Name = "Dominadas pronas", MuscleId = 1, Type = ExerciseType.Strength },
          new Exercise { Id = 3, Name = "Remo con barra", MuscleId = 1, Type = ExerciseType.Strength },
          new Exercise { Id = 4, Name = "Jalón al pecho agarre neutro", MuscleId = 1, Type = ExerciseType.Strength },
          new Exercise { Id = 5, Name = "Remo en polea baja", MuscleId = 1, Type = ExerciseType.Strength },
          new Exercise { Id = 6, Name = "Remo pecho apoyado en máquina", MuscleId = 1, Type = ExerciseType.Strength },

          // Biceps
          new Exercise { Id = 10, Name = "Curl con barra", MuscleId = 2, Type = ExerciseType.Strength },
          new Exercise { Id = 11, Name = "Curl en banco inclinado", MuscleId = 2, Type = ExerciseType.Strength },
          new Exercise { Id = 12, Name = "Curl en polea baja", MuscleId = 2, Type = ExerciseType.Strength },
          new Exercise { Id = 13, Name = "Predicador", MuscleId = 2, Type = ExerciseType.Strength },
          new Exercise { Id = 14, Name = "Curl martillo", MuscleId = 2, Type = ExerciseType.Strength },

          // Piernas
          new Exercise { Id = 20, Name = "Prensa de piernas", MuscleId = 3, Type = ExerciseType.Strength },
          new Exercise { Id = 21, Name = "Zancadas caminando", MuscleId = 3, Type = ExerciseType.Strength },
          new Exercise { Id = 22, Name = "Extensiones de piernas", MuscleId = 3, Type = ExerciseType.Strength },
          new Exercise { Id = 23, Name = "Elevaciones de talones de pie", MuscleId = 3, Type = ExerciseType.Strength },
          new Exercise { Id = 24, Name = "Hack squat", MuscleId = 3, Type = ExerciseType.Strength },
          new Exercise { Id = 25, Name = "Curl femoral sentado", MuscleId = 3, Type = ExerciseType.Strength },
          new Exercise { Id = 26, Name = "Curl femoral acostado", MuscleId = 3, Type = ExerciseType.Strength },
          new Exercise { Id = 27, Name = "Gemelos en prensa", MuscleId = 3, Type = ExerciseType.Strength },

          // Hombros
          new Exercise { Id = 30, Name = "Press militar con barra", MuscleId = 4, Type = ExerciseType.Strength },
          new Exercise { Id = 31, Name = "Face pull", MuscleId = 4, Type = ExerciseType.Strength },
          new Exercise { Id = 32, Name = "Arnold press", MuscleId = 4, Type = ExerciseType.Strength },
          new Exercise { Id = 33, Name = "Elevaciones laterales en polea", MuscleId = 4, Type = ExerciseType.Strength },
          new Exercise { Id = 34, Name = "Elevaciones posteriores en máquina", MuscleId = 4, Type = ExerciseType.Strength },

          // Pecho
          new Exercise { Id = 40, Name = "Press inclinado con mancuernas", MuscleId = 5, Type = ExerciseType.Strength },
          new Exercise { Id = 41, Name = "Press plano con barra", MuscleId = 5, Type = ExerciseType.Strength },
          new Exercise { Id = 42, Name = "Fondos en paralelas (pecho)", MuscleId = 5, Type = ExerciseType.Strength },
          new Exercise { Id = 43, Name = "Press inclinado en máquina", MuscleId = 5, Type = ExerciseType.Strength },
          new Exercise { Id = 44, Name = "Aperturas con mancuernas", MuscleId = 5, Type = ExerciseType.Strength },
          new Exercise { Id = 45, Name = "Cruce unilateral en polea", MuscleId = 5, Type = ExerciseType.Strength },

          // Triceps
          new Exercise { Id = 50, Name = "Press cerrado con barra", MuscleId = 6, Type = ExerciseType.Strength },
          new Exercise { Id = 51, Name = "Extensión por encima de la cabeza en polea con barra", MuscleId = 6, Type = ExerciseType.Strength },
          new Exercise { Id = 52, Name = "Extensión en polea con cuerda", MuscleId = 6, Type = ExerciseType.Strength },
          new Exercise { Id = 53, Name = "Rompecráneos", MuscleId = 6, Type = ExerciseType.Strength },

          // Abdomen
          new Exercise { Id = 60, Name = "Elevaciones de piernas colgado", MuscleId = 7, Type = ExerciseType.Strength },
          new Exercise { Id = 61, Name = "Plancha", MuscleId = 7, Type = ExerciseType.Strength },
          new Exercise { Id = 62, Name = "Ab wheel", MuscleId = 7, Type = ExerciseType.Strength },
          new Exercise { Id = 63, Name = "Crunch declinado", MuscleId = 7, Type = ExerciseType.Strength },
          new Exercise { Id = 64, Name = "Russian twist", MuscleId = 7, Type = ExerciseType.Strength },
          new Exercise { Id = 65, Name = "Plancha lateral", MuscleId = 7, Type = ExerciseType.Strength },

          // Cardio
          new Exercise { Id = 100, Name = "Caminadora", MuscleId = null, Type = ExerciseType.Cardio },
          new Exercise { Id = 101, Name = "Escalera Mecanica", MuscleId = null, Type = ExerciseType.Cardio }
      );
    }
  }
}