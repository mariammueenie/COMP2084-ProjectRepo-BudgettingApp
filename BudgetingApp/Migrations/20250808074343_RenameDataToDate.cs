using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetingApp.Migrations
{
    /// <inheritdoc />
    public partial class RenameDataToDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.RenameColumn(
        name: "Data",
        table: "Expenses",
        newName: "Date");
}


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
{
    migrationBuilder.RenameColumn(
        name: "Date",
        table: "Expenses",
        newName: "Data");
}

    }
}
