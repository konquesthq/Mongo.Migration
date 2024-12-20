﻿using FluentAssertions;
using Mongo.Migration.Documents;
using Mongo.Migration.Services;
using Mongo.Migration.Test.Core.Database;
using NUnit.Framework;

namespace Mongo.Migration.Test.Core;

[TestFixture]
internal class DatabaseVersionService_when_determine_version : DatabaseIntegrationTest
{
    private IDatabaseVersionService _service;

    protected override void OnSetUp(DocumentVersion version)
    {
        base.OnSetUp(version);

        _service = _components.Get<IDatabaseVersionService>();
    }

    [TearDown]
    public void TearDown()
    {
        Dispose();
    }

    [Test]
    public void When_project_has_migrations_Then_get_latest_version()
    {
        // Arrange 
        OnSetUp(DocumentVersion.Empty());

        // Act
        var migrationVersion = _service.GetCurrentOrLatestMigrationVersion();

        // Assert
        migrationVersion.ToString().Should().Be("0.0.3");
    }

    [Test]
    public void When_version_set_on_startup_Then_use_startup_version()
    {
        // Arrange 
        OnSetUp(new DocumentVersion(0, 0, 2));

        // Act
        var migrationVersion = _service.GetCurrentOrLatestMigrationVersion();

        // Assert
        migrationVersion.ToString().Should().Be("0.0.2");
    }
}