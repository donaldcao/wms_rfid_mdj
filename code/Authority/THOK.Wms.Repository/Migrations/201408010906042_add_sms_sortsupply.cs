namespace THOK.Wms.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_sms_sortsupply : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.sms_sort_supply",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        sort_batch_id = c.Int(nullable: false),
                        pack_no = c.Int(nullable: false),
                        channel_code = c.String(nullable: false, maxLength: 20),
                        product_code = c.String(maxLength: 20),
                        product_name = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.sms_channel", t => t.channel_code)
                .ForeignKey("dbo.sms_sort_batch", t => t.sort_batch_id)
                .Index(t => t.sort_batch_id)
                .Index(t => t.channel_code);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.sms_sort_supply", "sort_batch_id", "dbo.sms_sort_batch");
            DropForeignKey("dbo.sms_sort_supply", "channel_code", "dbo.sms_channel");
            DropIndex("dbo.sms_sort_supply", new[] { "channel_code" });
            DropIndex("dbo.sms_sort_supply", new[] { "sort_batch_id" });
            DropTable("dbo.sms_sort_supply");
        }
    }
}
