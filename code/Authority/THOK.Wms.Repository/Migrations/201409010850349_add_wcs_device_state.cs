namespace THOK.Wms.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_wcs_device_state : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.wcs_wcs_device_state",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        device_code = c.String(nullable: false, maxLength: 8),
                        device_name = c.String(nullable: false, maxLength: 50),
                        device_type = c.String(nullable: false, maxLength: 50),
                        state_code = c.String(nullable: false, maxLength: 8),
                        begin_time = c.DateTime(nullable: false),
                        end_time = c.DateTime(nullable: false),
                        use_time = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.wcs_wcs_device_state");
        }
    }
}
