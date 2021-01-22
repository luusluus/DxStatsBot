using DXStats.Extensions;
using DXStats.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using System;
using System.Reflection;

namespace DXStats.Domain.Overrides
{
    // https://medium.com/@pawel.gerr/entity-framework-core-changing-database-schema-at-runtime-dcf1211768c6
    public class SeederAwareMigrationsAssembly : MigrationsAssembly
    {
        private readonly DbContext _context;
        public SeederAwareMigrationsAssembly(ICurrentDbContext currentContext, IDbContextOptions options, IMigrationsIdGenerator idGenerator, IDiagnosticsLogger<DbLoggerCategory.Migrations> logger) : base(currentContext, options, idGenerator, logger)
        {
            _context = currentContext.Context;
        }

        public override Migration CreateMigration(TypeInfo migrationClass, string activeProvider)
        {
            var seeder = _context.GetSeeder();

            if (activeProvider == null)
                throw new ArgumentNullException(nameof(activeProvider));

            var hasCtorWithSchema = migrationClass
                    .GetConstructor(new[] { typeof(ISeeder) }) != null;

            if (hasCtorWithSchema)
            {
                var instance = (Migration)Activator.CreateInstance(migrationClass.AsType(), seeder);
                instance.ActiveProvider = activeProvider;
                return instance;
            }

            return base.CreateMigration(migrationClass, activeProvider);
        }
    }
}
