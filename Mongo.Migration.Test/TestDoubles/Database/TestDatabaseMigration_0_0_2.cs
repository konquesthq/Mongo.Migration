﻿using Mongo.Migration.Migrations.Database;
using MongoDB.Driver;

namespace Mongo.Migration.Test.Core.Database
{
    internal class TestDatabaseMigration_0_0_2 : DatabaseMigration
    {
        public TestDatabaseMigration_0_0_2()
            : base("0.0.2")
        {
        }

        public override void Up(IMongoDatabase db)
        {
        }

        public override void Down(IMongoDatabase db)
        {
        }
    }
}