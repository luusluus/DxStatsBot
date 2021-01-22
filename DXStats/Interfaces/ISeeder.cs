using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace DXStats.Interfaces
{
    public interface ISeeder : IDisposable
    {
        void Seed(MigrationBuilder migrationBuilder);
    }
}
