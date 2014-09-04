namespace THOK.Wms.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_status_to_sms_sort_supply : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.sms_sort_supply", "status", c => c.String(nullable: false, maxLength: 1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.sms_sort_supply", "status");
        }
    }
}
