using Orchard.Data.Migration;
using Orchard.ContentManagement.MetaData;

namespace MainBit.Alias
{
    public class Migrations: DataMigrationImpl {
        public int Create() {

            SchemaBuilder.CreateTable("UrlTemplateRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("Position")
                    .Column<string>("BaseUrl", c => c.WithLength(255))
                    .Column<string>("StoredPrefix", c => c.WithLength(255))
                    .Column<string>("Constraints", c => c.WithLength(2048))
                );

            SchemaBuilder.CreateTable("EnumUrlSegmentRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("Position")
                    .Column<string>("Name", c => c.WithLength(255))
                    .Column<string>("DisplayName", c => c.WithLength(255))
                );

            SchemaBuilder.CreateTable("EnumUrlSegmentValueRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("EnumUrlSegmentRecord_Id")
                    .Column<int>("Position")
                    .Column<string>("Name", c => c.WithLength(255))
                    .Column<string>("DisplayName", c => c.WithLength(255))
                    .Column<string>("UrlSegment", c => c.WithLength(255))
                    .Column<string>("StoredPrefix", c => c.WithLength(255))
                    .Column<bool>("IsDefault", c => c.WithLength(255))
                );

            return 5;
        }

        public int UpdateFrom5()
        {

            ContentDefinitionManager.AlterTypeDefinition("UrlSegmentWidget",
                cfg => cfg
                    .WithPart("IdentityPart")
                    .WithPart("CommonPart",
                        p => p
                            .WithSetting("OwnerEditorSettings.ShowOwnerEditor", "false")
                            .WithSetting("DateEditorSettings.ShowDateEditor", "false"))
                    .WithPart("WidgetPart")
                    .WithPart("ElementWrapperPart",
                        p => p
                            .WithSetting("ElementWrapperPartSettings.ElementTypeName", "MainBit.Alias.Elements.UrlSegment"))
                    .WithSetting("Stereotype", "Widget")
                );

            return 6;
        }

        public int UpdateFrom6()
        {
            SchemaBuilder.AlterTable("UrlTemplateRecord",
                table => table.AddColumn<bool>("IncludeDefaultValues", c => c.WithLength(2048)));

            return 7;
        }
    }
}