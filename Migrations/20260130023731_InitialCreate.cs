using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GymTracker.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Muscles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Muscles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DayOfWeek = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutDays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    MuscleId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercises_Muscles_MuscleId",
                        column: x => x.MuscleId,
                        principalTable: "Muscles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkoutDayMuscle",
                columns: table => new
                {
                    WorkoutDayId = table.Column<int>(type: "INTEGER", nullable: false),
                    MuscleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutDayMuscle", x => new { x.WorkoutDayId, x.MuscleId });
                    table.ForeignKey(
                        name: "FK_WorkoutDayMuscle_Muscles_MuscleId",
                        column: x => x.MuscleId,
                        principalTable: "Muscles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutDayMuscle_WorkoutDays_WorkoutDayId",
                        column: x => x.WorkoutDayId,
                        principalTable: "WorkoutDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    WorkoutDayId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutSessions_WorkoutDays_WorkoutDayId",
                        column: x => x.WorkoutDayId,
                        principalTable: "WorkoutDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutDayExercise",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkoutDayId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExerciseId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlannedSets = table.Column<int>(type: "INTEGER", nullable: false),
                    PlannedReps = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutDayExercise", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkoutDayExercise_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkoutDayExercise_WorkoutDays_WorkoutDayId",
                        column: x => x.WorkoutDayId,
                        principalTable: "WorkoutDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CardioSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkoutSessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExerciseId = table.Column<int>(type: "INTEGER", nullable: false),
                    DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    DistanceKm = table.Column<decimal>(type: "TEXT", nullable: true),
                    Calories = table.Column<int>(type: "INTEGER", nullable: true),
                    AvgHeartRate = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardioSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardioSessions_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardioSessions_WorkoutSessions_WorkoutSessionId",
                        column: x => x.WorkoutSessionId,
                        principalTable: "WorkoutSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkoutSessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExerciseId = table.Column<int>(type: "INTEGER", nullable: false),
                    SetNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Reps = table.Column<int>(type: "INTEGER", nullable: false),
                    Weight = table.Column<decimal>(type: "TEXT", nullable: false),
                    RIR = table.Column<int>(type: "INTEGER", nullable: true),
                    RestSeconds = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseSets_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExerciseSets_WorkoutSessions_WorkoutSessionId",
                        column: x => x.WorkoutSessionId,
                        principalTable: "WorkoutSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "MuscleId", "Name", "Type" },
                values: new object[,]
                {
                    { 100, null, "Caminadora", 2 },
                    { 101, null, "Escalera mecanica", 2 }
                });

            migrationBuilder.InsertData(
                table: "Muscles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Espalda" },
                    { 2, "Biceps" },
                    { 3, "Piernas" },
                    { 4, "Hombros" },
                    { 5, "Pecho" },
                    { 6, "Triceps" },
                    { 7, "Abdomen" }
                });

            migrationBuilder.InsertData(
                table: "WorkoutDays",
                columns: new[] { "Id", "DayOfWeek" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 },
                    { 4, 4 },
                    { 5, 5 },
                    { 6, 6 }
                });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "MuscleId", "Name", "Type" },
                values: new object[,]
                {
                    { 1, 1, "Peso muerto", 1 },
                    { 2, 1, "Dominadas pronas", 1 },
                    { 3, 1, "Remo con barra", 1 },
                    { 4, 1, "Jalon al pecho agarre neutro", 1 },
                    { 10, 2, "Curl con barra", 1 },
                    { 11, 2, "Curl en banco inclinado", 1 },
                    { 12, 2, "Curl martillo", 1 },
                    { 20, 3, "Prensa de piernas", 1 },
                    { 21, 3, "Hack squat", 1 },
                    { 22, 3, "Curl femoral acostado", 1 },
                    { 30, 4, "Press militar con barra", 1 },
                    { 31, 4, "Elevaciones laterales", 1 },
                    { 40, 5, "Press plano con barra", 1 },
                    { 41, 5, "Press inclinado con mancuernas", 1 },
                    { 50, 6, "Press cerrado con barra", 1 },
                    { 51, 6, "Extensión en polea con cuerda", 1 },
                    { 60, 7, "Plancha", 1 },
                    { 61, 7, "Ab wheel", 1 }
                });

            migrationBuilder.InsertData(
                table: "WorkoutDayMuscle",
                columns: new[] { "MuscleId", "WorkoutDayId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 2 },
                    { 4, 2 },
                    { 5, 3 },
                    { 6, 3 },
                    { 1, 4 },
                    { 2, 4 },
                    { 3, 5 },
                    { 4, 5 },
                    { 5, 6 },
                    { 6, 6 }
                });

            migrationBuilder.InsertData(
                table: "WorkoutDayExercise",
                columns: new[] { "Id", "ExerciseId", "PlannedReps", "PlannedSets", "WorkoutDayId" },
                values: new object[,]
                {
                    { 1, 1, 12, 3, 1 },
                    { 2, 2, 10, 3, 1 },
                    { 3, 3, 12, 3, 1 },
                    { 4, 4, 12, 3, 1 },
                    { 5, 10, 12, 3, 1 },
                    { 6, 11, 10, 3, 1 },
                    { 7, 12, 12, 3, 1 },
                    { 8, 20, 12, 3, 2 },
                    { 9, 21, 10, 3, 2 },
                    { 10, 22, 12, 3, 2 },
                    { 11, 30, 10, 3, 2 },
                    { 12, 31, 15, 3, 2 },
                    { 13, 40, 10, 3, 3 },
                    { 14, 41, 12, 3, 3 },
                    { 15, 50, 10, 3, 3 },
                    { 16, 51, 12, 3, 3 },
                    { 17, 3, 12, 3, 4 },
                    { 18, 4, 12, 3, 4 },
                    { 19, 10, 12, 3, 4 },
                    { 20, 12, 12, 3, 4 },
                    { 21, 20, 12, 3, 5 },
                    { 22, 21, 10, 3, 5 },
                    { 23, 22, 12, 3, 5 },
                    { 24, 31, 15, 3, 5 },
                    { 25, 40, 10, 3, 6 },
                    { 26, 41, 12, 3, 6 },
                    { 27, 50, 10, 3, 6 },
                    { 28, 51, 12, 3, 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CardioSessions_ExerciseId",
                table: "CardioSessions",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_CardioSessions_WorkoutSessionId",
                table: "CardioSessions",
                column: "WorkoutSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_MuscleId",
                table: "Exercises",
                column: "MuscleId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseSets_ExerciseId",
                table: "ExerciseSets",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseSets_WorkoutSessionId",
                table: "ExerciseSets",
                column: "WorkoutSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutDayExercise_ExerciseId",
                table: "WorkoutDayExercise",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutDayExercise_WorkoutDayId",
                table: "WorkoutDayExercise",
                column: "WorkoutDayId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutDayMuscle_MuscleId",
                table: "WorkoutDayMuscle",
                column: "MuscleId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_WorkoutDayId",
                table: "WorkoutSessions",
                column: "WorkoutDayId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardioSessions");

            migrationBuilder.DropTable(
                name: "ExerciseSets");

            migrationBuilder.DropTable(
                name: "WorkoutDayExercise");

            migrationBuilder.DropTable(
                name: "WorkoutDayMuscle");

            migrationBuilder.DropTable(
                name: "WorkoutSessions");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "WorkoutDays");

            migrationBuilder.DropTable(
                name: "Muscles");
        }
    }
}
