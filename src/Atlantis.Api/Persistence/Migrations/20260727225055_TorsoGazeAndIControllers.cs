using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlantis.Api.Migrations
{
    /// <inheritdoc />
    public partial class TorsoGazeAndIControllers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "gaze_direction_x",
                table: "entities",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "gaze_direction_y",
                table: "entities",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "gaze_direction_z",
                table: "entities",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "torso_front_x",
                table: "entities",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "torso_front_y",
                table: "entities",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "torso_front_z",
                table: "entities",
                type: "real",
                nullable: true);

            migrationBuilder.Sql(
    """
    UPDATE entities
    SET
        torso_front_x = 0,
        torso_front_y = 0,
        torso_front_z = 1,
        gaze_direction_x = 0,
        gaze_direction_y = 0,
        gaze_direction_z = 1
    WHERE type IN ('citizen', 'visitor')
      AND torso_front_x IS NULL
      AND torso_front_y IS NULL
      AND torso_front_z IS NULL
      AND gaze_direction_x IS NULL
      AND gaze_direction_y IS NULL
      AND gaze_direction_z IS NULL;
    """);

            migrationBuilder.AddCheckConstraint(
    name: "ck_entities_embodiment_all_or_none",
    table: "entities",
    sql:
        """
        (
            torso_front_x IS NULL
            AND torso_front_y IS NULL
            AND torso_front_z IS NULL
            AND gaze_direction_x IS NULL
            AND gaze_direction_y IS NULL
            AND gaze_direction_z IS NULL
        )
        OR
        (
            torso_front_x IS NOT NULL
            AND torso_front_y IS NOT NULL
            AND torso_front_z IS NOT NULL
            AND gaze_direction_x IS NOT NULL
            AND gaze_direction_y IS NOT NULL
            AND gaze_direction_z IS NOT NULL
        )
        """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
    name: "ck_entities_embodiment_all_or_none",
    table: "entities");

            migrationBuilder.DropColumn(
                name: "gaze_direction_x",
                table: "entities");

            migrationBuilder.DropColumn(
                name: "gaze_direction_y",
                table: "entities");

            migrationBuilder.DropColumn(
                name: "gaze_direction_z",
                table: "entities");

            migrationBuilder.DropColumn(
                name: "torso_front_x",
                table: "entities");

            migrationBuilder.DropColumn(
                name: "torso_front_y",
                table: "entities");

            migrationBuilder.DropColumn(
                name: "torso_front_z",
                table: "entities");
        }
    }
}
