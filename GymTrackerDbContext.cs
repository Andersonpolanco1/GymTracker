using GymTracker.Enums;
using GymTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace GymTracker
{

  public class GymTrackerDbContext(DbContextOptions<GymTrackerDbContext> options) : DbContext(options)
  {
    public DbSet<Muscle> Muscles => Set<Muscle>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<WorkoutDay> WorkoutDays => Set<WorkoutDay>();
    public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();
    public DbSet<ExerciseSet> ExerciseSets => Set<ExerciseSet>();
    public DbSet<CardioSession> CardioSessions => Set<CardioSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<WorkoutDayMuscle>()
    .HasKey(x => new { x.WorkoutDayId, x.MuscleId });

      modelBuilder.Entity<WorkoutDayMuscle>()
          .HasOne(x => x.WorkoutDay)
          .WithMany(d => d.Muscles)
          .HasForeignKey(x => x.WorkoutDayId);

      modelBuilder.Entity<WorkoutDayMuscle>()
          .HasOne(x => x.Muscle)
          .WithMany()
          .HasForeignKey(x => x.MuscleId);

      modelBuilder.Entity<WorkoutDayExercise>()
          .HasOne(x => x.WorkoutDay)
          .WithMany(d => d.Exercises)
          .HasForeignKey(x => x.WorkoutDayId);

      modelBuilder.Entity<WorkoutDayExercise>()
          .HasOne(x => x.Exercise)
          .WithMany()
          .HasForeignKey(x => x.ExerciseId);


      var muscles = new[]
      {
          new Muscle { Id = 1, Name = "Espalda" },
          new Muscle { Id = 2, Name = "Biceps" },
          new Muscle { Id = 3, Name = "Piernas" },
          new Muscle { Id = 4, Name = "Hombros" },
          new Muscle { Id = 5, Name = "Pecho" },
          new Muscle { Id = 6, Name = "Triceps" },
          new Muscle { Id = 7, Name = "Abdomen" }
      };

      modelBuilder.Entity<Muscle>().HasData(muscles);

      modelBuilder.Entity<Exercise>().HasData(
        new Exercise { Id = 1, Name = "Peso muerto", MuscleId = 1, Type = ExerciseType.Strength },
        new Exercise { Id = 2, Name = "Dominadas pronas", MuscleId = 1, Type = ExerciseType.Strength },
        new Exercise { Id = 3, Name = "Remo con barra", MuscleId = 1, Type = ExerciseType.Strength },
        new Exercise { Id = 4, Name = "Jalon al pecho agarre neutro", MuscleId = 1, Type = ExerciseType.Strength },

        new Exercise { Id = 10, Name = "Curl con barra", MuscleId = 2, Type = ExerciseType.Strength },
        new Exercise { Id = 11, Name = "Curl en banco inclinado", MuscleId = 2, Type = ExerciseType.Strength },
        new Exercise { Id = 12, Name = "Curl martillo", MuscleId = 2, Type = ExerciseType.Strength },

        new Exercise { Id = 20, Name = "Prensa de piernas", MuscleId = 3, Type = ExerciseType.Strength },
        new Exercise { Id = 21, Name = "Hack squat", MuscleId = 3, Type = ExerciseType.Strength },
        new Exercise { Id = 22, Name = "Curl femoral acostado", MuscleId = 3, Type = ExerciseType.Strength },

        new Exercise { Id = 30, Name = "Press militar con barra", MuscleId = 4, Type = ExerciseType.Strength },
        new Exercise { Id = 31, Name = "Elevaciones laterales", MuscleId = 4, Type = ExerciseType.Strength },

        new Exercise { Id = 40, Name = "Press plano con barra", MuscleId = 5, Type = ExerciseType.Strength },
        new Exercise { Id = 41, Name = "Press inclinado con mancuernas", MuscleId = 5, Type = ExerciseType.Strength },

        new Exercise { Id = 50, Name = "Press cerrado con barra", MuscleId = 6, Type = ExerciseType.Strength },
        new Exercise { Id = 51, Name = "Extensión en polea con cuerda", MuscleId = 6, Type = ExerciseType.Strength },

        new Exercise { Id = 60, Name = "Plancha", MuscleId = 7, Type = ExerciseType.Strength },
        new Exercise { Id = 61, Name = "Ab wheel", MuscleId = 7, Type = ExerciseType.Strength }
    );

      modelBuilder.Entity<Exercise>().HasData(
          new Exercise { Id = 100, Name = "Caminadora", Type = ExerciseType.Cardio },
          new Exercise { Id = 101, Name = "Escalera mecanica", Type = ExerciseType.Cardio }
      );

      modelBuilder.Entity<WorkoutDay>().HasData(
          new WorkoutDay { Id = 1, DayOfWeek = DayOfWeek.Monday },
          new WorkoutDay { Id = 2, DayOfWeek = DayOfWeek.Tuesday },
          new WorkoutDay { Id = 3, DayOfWeek = DayOfWeek.Wednesday },
          new WorkoutDay { Id = 4, DayOfWeek = DayOfWeek.Thursday },
          new WorkoutDay { Id = 5, DayOfWeek = DayOfWeek.Friday },
          new WorkoutDay { Id = 6, DayOfWeek = DayOfWeek.Saturday }
      );

      modelBuilder.Entity<WorkoutDayMuscle>().HasData(
          new { WorkoutDayId = 1, MuscleId = 1 },
          new { WorkoutDayId = 1, MuscleId = 2 },

          new { WorkoutDayId = 2, MuscleId = 3 },
          new { WorkoutDayId = 2, MuscleId = 4 },

          new { WorkoutDayId = 3, MuscleId = 5 },
          new { WorkoutDayId = 3, MuscleId = 6 },

          new { WorkoutDayId = 4, MuscleId = 1 },
          new { WorkoutDayId = 4, MuscleId = 2 },

          new { WorkoutDayId = 5, MuscleId = 3 },
          new { WorkoutDayId = 5, MuscleId = 4 },

          new { WorkoutDayId = 6, MuscleId = 5 },
          new { WorkoutDayId = 6, MuscleId = 6 }
      );


      modelBuilder.Entity<WorkoutDayExercise>().HasData(

          // =======================
          // LUNES – Espalda + Biceps
          // =======================
          new WorkoutDayExercise { Id = 1, WorkoutDayId = 1, ExerciseId = 1, PlannedSets = 3, PlannedReps = 12 }, // Peso muerto
          new WorkoutDayExercise { Id = 2, WorkoutDayId = 1, ExerciseId = 2, PlannedSets = 3, PlannedReps = 10 }, // Dominadas
          new WorkoutDayExercise { Id = 3, WorkoutDayId = 1, ExerciseId = 3, PlannedSets = 3, PlannedReps = 12 }, // Remo barra
          new WorkoutDayExercise { Id = 4, WorkoutDayId = 1, ExerciseId = 4, PlannedSets = 3, PlannedReps = 12 }, // Jalón pecho

          new WorkoutDayExercise { Id = 5, WorkoutDayId = 1, ExerciseId = 10, PlannedSets = 3, PlannedReps = 12 }, // Curl barra
          new WorkoutDayExercise { Id = 6, WorkoutDayId = 1, ExerciseId = 11, PlannedSets = 3, PlannedReps = 10 }, // Curl inclinado
          new WorkoutDayExercise { Id = 7, WorkoutDayId = 1, ExerciseId = 12, PlannedSets = 3, PlannedReps = 12 }, // Curl martillo

          // =======================
          // MARTES – Piernas + Hombros
          // =======================
          new WorkoutDayExercise { Id = 8, WorkoutDayId = 2, ExerciseId = 20, PlannedSets = 3, PlannedReps = 12 }, // Prensa
          new WorkoutDayExercise { Id = 9, WorkoutDayId = 2, ExerciseId = 21, PlannedSets = 3, PlannedReps = 10 }, // Hack squat
          new WorkoutDayExercise { Id = 10, WorkoutDayId = 2, ExerciseId = 22, PlannedSets = 3, PlannedReps = 12 }, // Curl femoral

          new WorkoutDayExercise { Id = 11, WorkoutDayId = 2, ExerciseId = 30, PlannedSets = 3, PlannedReps = 10 }, // Press militar
          new WorkoutDayExercise { Id = 12, WorkoutDayId = 2, ExerciseId = 31, PlannedSets = 3, PlannedReps = 15 }, // Laterales

          // =======================
          // MIÉRCOLES – Pecho + Triceps
          // =======================
          new WorkoutDayExercise { Id = 13, WorkoutDayId = 3, ExerciseId = 40, PlannedSets = 3, PlannedReps = 10 }, // Press plano
          new WorkoutDayExercise { Id = 14, WorkoutDayId = 3, ExerciseId = 41, PlannedSets = 3, PlannedReps = 12 }, // Press inclinado

          new WorkoutDayExercise { Id = 15, WorkoutDayId = 3, ExerciseId = 50, PlannedSets = 3, PlannedReps = 10 }, // Press cerrado
          new WorkoutDayExercise { Id = 16, WorkoutDayId = 3, ExerciseId = 51, PlannedSets = 3, PlannedReps = 12 }, // Extensión polea

          // =======================
          // JUEVES – Espalda + Biceps
          // =======================
          new WorkoutDayExercise { Id = 17, WorkoutDayId = 4, ExerciseId = 3, PlannedSets = 3, PlannedReps = 12 }, // Remo barra
          new WorkoutDayExercise { Id = 18, WorkoutDayId = 4, ExerciseId = 4, PlannedSets = 3, PlannedReps = 12 }, // Jalón

          new WorkoutDayExercise { Id = 19, WorkoutDayId = 4, ExerciseId = 10, PlannedSets = 3, PlannedReps = 12 }, // Curl barra
          new WorkoutDayExercise { Id = 20, WorkoutDayId = 4, ExerciseId = 12, PlannedSets = 3, PlannedReps = 12 }, // Curl martillo

          // =======================
          // VIERNES – Piernas + Hombros
          // =======================
          new WorkoutDayExercise { Id = 21, WorkoutDayId = 5, ExerciseId = 20, PlannedSets = 3, PlannedReps = 12 }, // Prensa
          new WorkoutDayExercise { Id = 22, WorkoutDayId = 5, ExerciseId = 21, PlannedSets = 3, PlannedReps = 10 }, // Hack squat
          new WorkoutDayExercise { Id = 23, WorkoutDayId = 5, ExerciseId = 22, PlannedSets = 3, PlannedReps = 12 }, // Curl femoral

          new WorkoutDayExercise { Id = 24, WorkoutDayId = 5, ExerciseId = 31, PlannedSets = 3, PlannedReps = 15 }, // Laterales

          // =======================
          // SÁBADO – Pecho + Triceps
          // =======================
          new WorkoutDayExercise { Id = 25, WorkoutDayId = 6, ExerciseId = 40, PlannedSets = 3, PlannedReps = 10 }, // Press plano
          new WorkoutDayExercise { Id = 26, WorkoutDayId = 6, ExerciseId = 41, PlannedSets = 3, PlannedReps = 12 }, // Press inclinado

          new WorkoutDayExercise { Id = 27, WorkoutDayId = 6, ExerciseId = 50, PlannedSets = 3, PlannedReps = 10 }, // Press cerrado
          new WorkoutDayExercise { Id = 28, WorkoutDayId = 6, ExerciseId = 51, PlannedSets = 3, PlannedReps = 12 }  // Extensión polea
      );


    }

  }

}
