using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Migrations.Document;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services
{
    internal class MigrationService : IMigrationService
    {
        private readonly ILogger<MigrationService> _logger;

        private readonly DocumentVersionSerializer _serializer;

        private readonly IStartUpDatabaseMigrationRunner _startUpDatabaseMigrationRunner;

        private readonly IStartUpDocumentMigrationRunner _startUpDocumentMigrationRunner;

        public MigrationService(
            DocumentVersionSerializer serializer,
            IStartUpDocumentMigrationRunner startUpDocumentMigrationRunner,
            IStartUpDatabaseMigrationRunner startUpDatabaseMigrationRunner)
            : this(serializer, NullLoggerFactory.Instance)
        {
            this._startUpDocumentMigrationRunner = startUpDocumentMigrationRunner;
            this._startUpDatabaseMigrationRunner = startUpDatabaseMigrationRunner;
        }

        private MigrationService(
            DocumentVersionSerializer serializer,
            ILoggerFactory loggerFactory)
        {
            this._serializer = serializer;
            this._logger = loggerFactory.CreateLogger<MigrationService>();
        }

        public void Migrate()
        {
            this.RegisterSerializer();
            this.OnStartup();
        }

        private void OnStartup()
        {
            this._startUpDatabaseMigrationRunner.RunAll();
            this._startUpDocumentMigrationRunner.RunAll();
        }

        private void RegisterSerializer()
        {
            try
            {
                BsonSerializer.RegisterSerializer(this._serializer.ValueType, this._serializer);
            }
            catch (BsonSerializationException ex)
            {
                // Catch if Serializer was registered already ... not great, I know.
                // We have to do this, because there is always a default DocumentVersionSerialzer.
                // BsonSerializer.LookupSerializer(), does not work.

                this._logger.LogError(ex, ex.GetType().ToString());
            }
        }
    }
}