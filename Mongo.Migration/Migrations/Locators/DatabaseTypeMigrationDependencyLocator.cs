using System;
using System.Collections.Generic;

using Mongo.Migration.Migrations.Adapters;
using Mongo.Migration.Migrations.Database;

namespace Mongo.Migration.Migrations.Locators;

internal class DatabaseTypeMigrationDependencyLocator(IContainerProvider containerProvider)
    : TypeMigrationDependencyLocator<IDatabaseMigration>(containerProvider), IDatabaseTypeMigrationDependencyLocator
{
    private IDictionary<Type, IReadOnlyCollection<IDatabaseMigration>> _migrations;

    protected override IDictionary<Type, IReadOnlyCollection<IDatabaseMigration>> Migrations
    {
        get
        {
            if (_migrations == null)
            {
                Locate();
            }

            return _migrations;
        }
        set => _migrations = value;
    }
}