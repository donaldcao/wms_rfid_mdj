namespace THOK.Wms.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_Channel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.sms_channel", "supply_cache_position", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.sms_channel", "supply_cache_position");
        }
    }
}
