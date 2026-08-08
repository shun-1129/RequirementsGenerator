using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequirementsDBEntity.Migrations
{
    /// <inheritdoc />
    public partial class Ver001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "m_requirements_definition",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false, comment: "要求事項定義ID"),
                    requirement_definition = table.Column<string>(type: "TEXT", nullable: false, comment: "要求事項定義:【値例】F：機能要求 , NF：非機能要求"),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    create_user = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    create_program = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    updated_at = table.Column<DateTime>(type: "TEXT", maxLength: 128, nullable: false),
                    update_user = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    update_program = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("m_requirements_definition_pkc", x => x.id);
                },
                comment: "要求事項定義マスタ");

            migrationBuilder.CreateTable(
                name: "t_requirement_id",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false, comment: "要求事項ID")
                        .Annotation("Sqlite:Autoincrement", true),
                    requirement_definition_id = table.Column<int>(type: "INTEGER", nullable: false, comment: "要求事項定義マスタID"),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    create_user = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    create_program = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    updated_at = table.Column<DateTime>(type: "TEXT", maxLength: 128, nullable: false),
                    update_user = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    update_program = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("t_requirement_id_pkc", x => x.id);
                },
                comment: "要求事項IDテーブル");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "m_requirements_definition");

            migrationBuilder.DropTable(
                name: "t_requirement_id");
        }
    }
}
