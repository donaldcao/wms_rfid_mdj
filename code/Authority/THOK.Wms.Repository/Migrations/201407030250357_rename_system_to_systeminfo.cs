namespace THOK.Wms.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rename_system_to_systeminfo : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.auth_system", newName: "auth_system_info");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.auth_system_info", newName: "auth_system");
        }
    }
}
