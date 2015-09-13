using Orchard.Data.Migration;

namespace MainBit.Alias
{
    public class Migrations: DataMigrationImpl {
        public int Create() {
            SchemaBuilder.CreateTable("BaseUrlTemplateRecord", 
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("Position")
                    .Column<string>("BaseUrlTemplate", c => c.WithLength(255))
                    .Column<string>("StoredVirtualPathTemplate", c => c.WithLength(255))
                );

            return 1;
        }
    }
}