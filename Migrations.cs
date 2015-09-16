using Orchard.Data.Migration;

namespace MainBit.Alias
{
    public class Migrations: DataMigrationImpl {
        public int Create() {
            SchemaBuilder.CreateTable("UrlTemplateRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("Position")
                    .Column<string>("BaseUrl", c => c.WithLength(255))
                    .Column<string>("StoredVirtualPath", c => c.WithLength(255))
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.CreateTable("EnumUrlSegmentRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("Position")
                    .Column<string>("Name", c => c.WithLength(255))
                    .Column<string>("PossibleValues", c => c.WithLength(255))
                    .Column<string>("DefaultValue", c => c.WithLength(255))
                );

            return 2;
        }
    }
}