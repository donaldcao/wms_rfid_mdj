namespace THOK.Wms.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_isacceptremainquantity_to_sms_channel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.sms_channel", "is_accept_remain_quantity", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.sms_channel", "is_accept_remain_quantity");
        }
    }
}
