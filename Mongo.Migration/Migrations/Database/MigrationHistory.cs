using Mongo.Migration.Documents;

namespace Mongo.Migration.Migrations.Database
{
    public class MigrationHistory
    {
        public string MigrationId { get; set; }
        public DocumentVersion Version { get; set; }
    }
}
