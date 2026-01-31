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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Muscles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkoutDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkoutDays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    MuscleId = table.Column<int>(type: "int", nullable: true)
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
                name: "WorkoutSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkoutDayId = table.Column<int>(type: "int", nullable: false)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkoutDayId = table.Column<int>(type: "int", nullable: false),
                    ExerciseId = table.Column<int>(type: "int", nullable: false),
                    PlannedSets = table.Column<int>(type: "int", nullable: false),
                    PlannedReps = table.Column<int>(type: "int", nullable: false)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkoutSessionId = table.Column<int>(type: "int", nullable: false),
                    ExerciseId = table.Column<int>(type: "int", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    DistanceKm = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Calories = table.Column<int>(type: "int", nullable: true),
                    AvgHeartRate = table.Column<int>(type: "int", nullable: true)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkoutSessionId = table.Column<int>(type: "int", nullable: false),
                    ExerciseId = table.Column<int>(type: "int", nullable: false),
                    SetNumber = table.Column<int>(type: "int", nullable: false),
                    Reps = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RIR = table.Column<int>(type: "int", nullable: true),
                    RestSeconds = table.Column<int>(type: "int", nullable: true)
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
                    { 101, null, "Escalera Mecanica", 2 }
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
                    { 1, 1, "Peso muerto (espalda baja)", 1 },
                    { 2, 1, "Dominadas pronas", 1 },
                    { 3, 1, "Remo con barra", 1 },
                    { 4, 1, "Jalón al pecho agarre neutro", 1 },
                    { 5, 1, "Remo en polea baja", 1 },
                    { 6, 1, "Remo pecho apoyado en máquina", 1 },
                    { 10, 2, "Curl con barra", 1 },
                    { 11, 2, "Curl en banco inclinado", 1 },
                    { 12, 2, "Curl en polea baja", 1 },
                    { 13, 2, "Predicador", 1 },
                    { 14, 2, "Curl martillo", 1 },
                    { 20, 3, "Prensa de piernas", 1 },
                    { 21, 3, "Zancadas caminando", 1 },
                    { 22, 3, "Extensiones de piernas", 1 },
                    { 23, 3, "Elevaciones de talones de pie", 1 },
                    { 24, 3, "Hack squat", 1 },
                    { 25, 3, "Curl femoral sentado", 1 },
                    { 26, 3, "Curl femoral acostado", 1 },
                    { 27, 3, "Gemelos en prensa", 1 },
                    { 30, 4, "Press militar con barra", 1 },
                    { 31, 4, "Face pull", 1 },
                    { 32, 4, "Arnold press", 1 },
                    { 33, 4, "Elevaciones laterales en polea", 1 },
                    { 34, 4, "Elevaciones posteriores en máquina", 1 },
                    { 40, 5, "Press inclinado con mancuernas", 1 },
                    { 41, 5, "Press plano con barra", 1 },
                    { 42, 5, "Fondos en paralelas (pecho)", 1 },
                    { 43, 5, "Press inclinado en máquina", 1 },
                    { 44, 5, "Aperturas con mancuernas", 1 },
                    { 45, 5, "Cruce unilateral en polea", 1 },
                    { 50, 6, "Press cerrado con barra", 1 },
                    { 51, 6, "Extensión por encima de la cabeza en polea con barra", 1 },
                    { 52, 6, "Extensión en polea con cuerda", 1 },
                    { 53, 6, "Rompecráneos", 1 },
                    { 60, 7, "Elevaciones de piernas colgado", 1 },
                    { 61, 7, "Plancha", 1 },
                    { 62, 7, "Ab wheel", 1 },
                    { 63, 7, "Crunch declinado", 1 },
                    { 64, 7, "Russian twist", 1 },
                    { 65, 7, "Plancha lateral", 1 }
                });

            migrationBuilder.InsertData(
                table: "WorkoutDayExercise",
                columns: new[] { "Id", "ExerciseId", "PlannedReps", "PlannedSets", "WorkoutDayId" },
                values: new object[,]
                {
                    { 6, 101, 15, 1, 1 },
                    { 13, 100, 15, 1, 2 },
                    { 28, 101, 15, 1, 4 },
                    { 36, 100, 15, 1, 5 },
                    { 1, 1, 12, 3, 1 },
                    { 2, 2, 10, 3, 1 },
                    { 3, 3, 12, 3, 1 },
                    { 4, 10, 12, 3, 1 },
                    { 5, 11, 10, 3, 1 },
                    { 7, 20, 12, 3, 2 },
                    { 8, 21, 12, 3, 2 },
                    { 9, 22, 12, 3, 2 },
                    { 10, 23, 15, 3, 2 },
                    { 11, 30, 10, 3, 2 },
                    { 12, 31, 12, 3, 2 },
                    { 14, 40, 12, 3, 3 },
                    { 15, 41, 10, 3, 3 },
                    { 16, 42, 12, 3, 3 },
                    { 17, 50, 12, 3, 3 },
                    { 18, 51, 12, 3, 3 },
                    { 19, 60, 15, 3, 3 },
                    { 20, 61, 60, 3, 3 },
                    { 21, 62, 12, 3, 3 },
                    { 22, 4, 12, 3, 4 },
                    { 23, 5, 12, 3, 4 },
                    { 24, 6, 12, 3, 4 },
                    { 25, 12, 12, 3, 4 },
                    { 26, 13, 12, 3, 4 },
                    { 27, 14, 12, 3, 4 },
                    { 29, 24, 12, 3, 5 },
                    { 30, 25, 12, 3, 5 },
                    { 31, 26, 12, 3, 5 },
                    { 32, 27, 15, 3, 5 },
                    { 33, 32, 10, 3, 5 },
                    { 34, 33, 12, 3, 5 },
                    { 35, 34, 12, 3, 5 },
                    { 37, 43, 12, 3, 6 },
                    { 38, 44, 12, 3, 6 },
                    { 39, 45, 12, 3, 6 },
                    { 40, 52, 12, 3, 6 },
                    { 41, 53, 12, 3, 6 },
                    { 42, 63, 12, 3, 6 },
                    { 43, 64, 12, 3, 6 },
                    { 44, 65, 60, 3, 6 }
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
