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



      modelBuilder.Entity<WorkoutDay>().HasData(
          new WorkoutDay { Id = 1, DayOfWeek = DayOfWeek.Monday },
          new WorkoutDay { Id = 2, DayOfWeek = DayOfWeek.Tuesday },
          new WorkoutDay { Id = 3, DayOfWeek = DayOfWeek.Wednesday },
          new WorkoutDay { Id = 4, DayOfWeek = DayOfWeek.Thursday },
          new WorkoutDay { Id = 5, DayOfWeek = DayOfWeek.Friday },
          new WorkoutDay { Id = 6, DayOfWeek = DayOfWeek.Saturday }
      );


      modelBuilder.Entity<WorkoutDayExercise>().HasData(

          // =======================
          // LUNES – Espalda + Biceps + Cardio
          // =======================
          new WorkoutDayExercise { Id = 1, WorkoutDayId = 1, ExerciseId = 1, PlannedSets = 3, PlannedReps = 12 }, // Peso muerto
          new WorkoutDayExercise { Id = 2, WorkoutDayId = 1, ExerciseId = 2, PlannedSets = 3, PlannedReps = 10 }, // Dominadas
          new WorkoutDayExercise { Id = 3, WorkoutDayId = 1, ExerciseId = 3, PlannedSets = 3, PlannedReps = 12 }, // Remo barra
          new WorkoutDayExercise { Id = 4, WorkoutDayId = 1, ExerciseId = 10, PlannedSets = 3, PlannedReps = 12 }, // Curl barra
          new WorkoutDayExercise { Id = 5, WorkoutDayId = 1, ExerciseId = 11, PlannedSets = 3, PlannedReps = 10 }, // Curl inclinado
          new WorkoutDayExercise { Id = 6, WorkoutDayId = 1, ExerciseId = 101, PlannedSets = 1, PlannedReps = 15 }, // Escalera mecanica

          // =======================
          // MARTES – Piernas + Hombros + Cardio
          // =======================
          new WorkoutDayExercise { Id = 7, WorkoutDayId = 2, ExerciseId = 20, PlannedSets = 3, PlannedReps = 12 }, // Prensa piernas
          new WorkoutDayExercise { Id = 8, WorkoutDayId = 2, ExerciseId = 21, PlannedSets = 3, PlannedReps = 12 }, // Zancadas caminando
          new WorkoutDayExercise { Id = 9, WorkoutDayId = 2, ExerciseId = 22, PlannedSets = 3, PlannedReps = 12 }, // Extensiones piernas
          new WorkoutDayExercise { Id = 10, WorkoutDayId = 2, ExerciseId = 23, PlannedSets = 3, PlannedReps = 15 }, // Elevaciones talones
          new WorkoutDayExercise { Id = 11, WorkoutDayId = 2, ExerciseId = 30, PlannedSets = 3, PlannedReps = 10 }, // Press militar
          new WorkoutDayExercise { Id = 12, WorkoutDayId = 2, ExerciseId = 31, PlannedSets = 3, PlannedReps = 12 }, // Face pull
          new WorkoutDayExercise { Id = 13, WorkoutDayId = 2, ExerciseId = 100, PlannedSets = 1, PlannedReps = 15 }, // Caminadora

          // =======================
          // MIÉRCOLES – Pecho + Triceps + Abdomen
          // =======================
          new WorkoutDayExercise { Id = 14, WorkoutDayId = 3, ExerciseId = 40, PlannedSets = 3, PlannedReps = 12 }, // Press inclinado mancuernas
          new WorkoutDayExercise { Id = 15, WorkoutDayId = 3, ExerciseId = 41, PlannedSets = 3, PlannedReps = 10 }, // Press plano
          new WorkoutDayExercise { Id = 16, WorkoutDayId = 3, ExerciseId = 42, PlannedSets = 3, PlannedReps = 12 }, // Fondos pecho
          new WorkoutDayExercise { Id = 17, WorkoutDayId = 3, ExerciseId = 50, PlannedSets = 3, PlannedReps = 12 }, // Press cerrado barra
          new WorkoutDayExercise { Id = 18, WorkoutDayId = 3, ExerciseId = 51, PlannedSets = 3, PlannedReps = 12 }, // Extensión polea
          new WorkoutDayExercise { Id = 19, WorkoutDayId = 3, ExerciseId = 60, PlannedSets = 3, PlannedReps = 15 }, // Elevaciones piernas colgado
          new WorkoutDayExercise { Id = 20, WorkoutDayId = 3, ExerciseId = 61, PlannedSets = 3, PlannedReps = 60 }, // Plancha segundos
          new WorkoutDayExercise { Id = 21, WorkoutDayId = 3, ExerciseId = 62, PlannedSets = 3, PlannedReps = 12 }, // Ab wheel

          // =======================
          // JUEVES – Espalda + Biceps + Cardio
          // =======================
          new WorkoutDayExercise { Id = 22, WorkoutDayId = 4, ExerciseId = 4, PlannedSets = 3, PlannedReps = 12 }, // Jalón pecho
          new WorkoutDayExercise { Id = 23, WorkoutDayId = 4, ExerciseId = 5, PlannedSets = 3, PlannedReps = 12 }, // Remo polea baja
          new WorkoutDayExercise { Id = 24, WorkoutDayId = 4, ExerciseId = 6, PlannedSets = 3, PlannedReps = 12 }, // Remo pecho apoyado máquina
          new WorkoutDayExercise { Id = 25, WorkoutDayId = 4, ExerciseId = 12, PlannedSets = 3, PlannedReps = 12 }, // Curl polea baja
          new WorkoutDayExercise { Id = 26, WorkoutDayId = 4, ExerciseId = 13, PlannedSets = 3, PlannedReps = 12 }, // Predicador
          new WorkoutDayExercise { Id = 27, WorkoutDayId = 4, ExerciseId = 14, PlannedSets = 3, PlannedReps = 12 }, // Curl martillo
          new WorkoutDayExercise { Id = 28, WorkoutDayId = 4, ExerciseId = 101, PlannedSets = 1, PlannedReps = 15 }, // Escalera mecanica

          // =======================
          // VIERNES – Piernas + Hombros + Cardio
          // =======================
          new WorkoutDayExercise { Id = 29, WorkoutDayId = 5, ExerciseId = 24, PlannedSets = 3, PlannedReps = 12 }, // Hack squat
          new WorkoutDayExercise { Id = 30, WorkoutDayId = 5, ExerciseId = 25, PlannedSets = 3, PlannedReps = 12 }, // Curl femoral sentado
          new WorkoutDayExercise { Id = 31, WorkoutDayId = 5, ExerciseId = 26, PlannedSets = 3, PlannedReps = 12 }, // Curl femoral acostado
          new WorkoutDayExercise { Id = 32, WorkoutDayId = 5, ExerciseId = 27, PlannedSets = 3, PlannedReps = 15 }, // Gemelos prensa
          new WorkoutDayExercise { Id = 33, WorkoutDayId = 5, ExerciseId = 32, PlannedSets = 3, PlannedReps = 10 }, // Arnold press
          new WorkoutDayExercise { Id = 34, WorkoutDayId = 5, ExerciseId = 33, PlannedSets = 3, PlannedReps = 12 }, // Elevaciones laterales polea
          new WorkoutDayExercise { Id = 35, WorkoutDayId = 5, ExerciseId = 34, PlannedSets = 3, PlannedReps = 12 }, // Elevaciones posteriores máquina
          new WorkoutDayExercise { Id = 36, WorkoutDayId = 5, ExerciseId = 100, PlannedSets = 1, PlannedReps = 15 }, // Caminadora

          // =======================
          // SÁBADO – Pecho + Triceps + Abdomen
          // =======================
          new WorkoutDayExercise { Id = 37, WorkoutDayId = 6, ExerciseId = 43, PlannedSets = 3, PlannedReps = 12 }, // Press inclinado máquina
          new WorkoutDayExercise { Id = 38, WorkoutDayId = 6, ExerciseId = 44, PlannedSets = 3, PlannedReps = 12 }, // Aperturas mancuernas
          new WorkoutDayExercise { Id = 39, WorkoutDayId = 6, ExerciseId = 45, PlannedSets = 3, PlannedReps = 12 }, // Cruce polea
          new WorkoutDayExercise { Id = 40, WorkoutDayId = 6, ExerciseId = 52, PlannedSets = 3, PlannedReps = 12 }, // Extensión cuerda
          new WorkoutDayExercise { Id = 41, WorkoutDayId = 6, ExerciseId = 53, PlannedSets = 3, PlannedReps = 12 }, // Rompecráneos
          new WorkoutDayExercise { Id = 42, WorkoutDayId = 6, ExerciseId = 63, PlannedSets = 3, PlannedReps = 12 }, // Crunch declinado
          new WorkoutDayExercise { Id = 43, WorkoutDayId = 6, ExerciseId = 64, PlannedSets = 3, PlannedReps = 12 }, // Russian twist
          new WorkoutDayExercise { Id = 44, WorkoutDayId = 6, ExerciseId = 65, PlannedSets = 3, PlannedReps = 60 }  // Plancha lateral segundos
      );
    }
  }
}
