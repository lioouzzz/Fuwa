using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fuwa.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGrammarExampleFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExampleSentence",
                table: "Grammars");

            migrationBuilder.DropColumn(
                name: "ExampleTranslation",
                table: "Grammars");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExampleSentence",
                table: "Grammars",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExampleTranslation",
                table: "Grammars",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
