namespace THOK.Wms.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_new_sms_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.sms_sort_batch",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        order_date = c.DateTime(nullable: false),
                        batch_no = c.Int(nullable: false),
                        sorting_line_code = c.String(nullable: false, maxLength: 20),
                        no_one_batch_no = c.Int(nullable: false),
                        sort_date = c.DateTime(nullable: false),
                        status = c.String(nullable: false, maxLength: 2, fixedLength: true),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.sms_channel_allot",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        sort_batch_id = c.Int(nullable: false),
                        channel_code = c.String(nullable: false, maxLength: 20),
                        product_code = c.String(maxLength: 20),
                        product_name = c.String(maxLength: 50),
                        quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.sms_channel", t => t.channel_code)
                .ForeignKey("dbo.sms_sort_batch", t => t.sort_batch_id)
                .Index(t => t.sort_batch_id)
                .Index(t => t.channel_code);
            
            CreateTable(
                "dbo.sms_channel",
                c => new
                    {
                        channel_code = c.String(nullable: false, maxLength: 20),
                        channel_type = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        channel_name = c.String(nullable: false, maxLength: 100),
                        sorting_line_code = c.String(nullable: false, maxLength: 20),
                        led_no = c.Int(nullable: false),
                        x = c.Int(nullable: false),
                        y = c.Int(nullable: false),
                        width = c.Int(nullable: false),
                        height = c.Int(nullable: false),
                        default_product_code = c.String(maxLength: 20),
                        default_product_name = c.String(maxLength: 50),
                        remain_quantity = c.Int(nullable: false),
                        channel_capacity = c.Int(nullable: false),
                        group_no = c.Int(nullable: false),
                        order_no = c.Int(nullable: false),
                        sort_address = c.Int(nullable: false),
                        supply_address = c.Int(nullable: false),
                        status = c.String(nullable: false, maxLength: 2, fixedLength: true),
                    })
                .PrimaryKey(t => t.channel_code);
            
            CreateTable(
                "dbo.sms_hand_supply",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        sort_batch_id = c.Int(nullable: false),
                        supply_id = c.Int(nullable: false),
                        supply_batch = c.Int(nullable: false),
                        pack_no = c.Int(nullable: false),
                        channel_code = c.String(nullable: false, maxLength: 20),
                        product_code = c.String(maxLength: 20),
                        product_name = c.String(maxLength: 50),
                        quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.sms_channel", t => t.channel_code)
                .ForeignKey("dbo.sms_sort_batch", t => t.sort_batch_id)
                .Index(t => t.sort_batch_id)
                .Index(t => t.channel_code);
            
            CreateTable(
                "dbo.sms_sort_order_allot_detail",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        master_id = c.Int(nullable: false),
                        channel_code = c.String(nullable: false, maxLength: 20),
                        product_code = c.String(nullable: false, maxLength: 20),
                        product_name = c.String(nullable: false, maxLength: 50),
                        quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.sms_channel", t => t.channel_code)
                .ForeignKey("dbo.sms_sort_order_allot_master", t => t.master_id)
                .Index(t => t.master_id)
                .Index(t => t.channel_code);
            
            CreateTable(
                "dbo.sms_sort_order_allot_master",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        sort_batch_id = c.Int(nullable: false),
                        pack_no = c.Int(nullable: false),
                        order_id = c.String(nullable: false, maxLength: 20),
                        deliver_line_code = c.String(nullable: false, maxLength: 50),
                        customer_code = c.String(maxLength: 50),
                        customer_name = c.String(nullable: false, maxLength: 100),
                        customer_order = c.Int(nullable: false),
                        customer_deliver_order = c.Int(nullable: false),
                        customer_info = c.String(nullable: false),
                        quantity = c.Int(nullable: false),
                        export_no = c.Int(nullable: false),
                        start_time = c.DateTime(nullable: false),
                        finish_time = c.DateTime(nullable: false),
                        status = c.String(nullable: false, maxLength: 2, fixedLength: true),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.sms_sort_batch", t => t.sort_batch_id)
                .Index(t => t.sort_batch_id);
            
            AddColumn("dbo.wms_sorting_line", "product_type", c => c.String(nullable: false, maxLength: 1, fixedLength: true));
            AddColumn("dbo.wms_sort_order_dispatch", "batch_sort_id", c => c.Int(nullable: false));
            AddColumn("dbo.wms_sort_order_dispatch", "deliver_line_no", c => c.Int(nullable: false));
            AddColumn("dbo.wms_sort_order_dispatch", "sort_status", c => c.String(maxLength: 1, fixedLength: true));
            AddColumn("dbo.wms_deliver_dist", "deliver_order", c => c.Int(nullable: false));
            AlterColumn("dbo.wms_sorting_line", "sorting_line_type", c => c.String(maxLength: 1, fixedLength: true));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.sms_channel_allot", "sort_batch_id", "dbo.sms_sort_batch");
            DropForeignKey("dbo.sms_channel_allot", "channel_code", "dbo.sms_channel");
            DropForeignKey("dbo.sms_sort_order_allot_detail", "master_id", "dbo.sms_sort_order_allot_master");
            DropForeignKey("dbo.sms_sort_order_allot_master", "sort_batch_id", "dbo.sms_sort_batch");
            DropForeignKey("dbo.sms_sort_order_allot_detail", "channel_code", "dbo.sms_channel");
            DropForeignKey("dbo.sms_hand_supply", "sort_batch_id", "dbo.sms_sort_batch");
            DropForeignKey("dbo.sms_hand_supply", "channel_code", "dbo.sms_channel");
            DropIndex("dbo.sms_sort_order_allot_master", new[] { "sort_batch_id" });
            DropIndex("dbo.sms_sort_order_allot_detail", new[] { "channel_code" });
            DropIndex("dbo.sms_sort_order_allot_detail", new[] { "master_id" });
            DropIndex("dbo.sms_hand_supply", new[] { "channel_code" });
            DropIndex("dbo.sms_hand_supply", new[] { "sort_batch_id" });
            DropIndex("dbo.sms_channel_allot", new[] { "channel_code" });
            DropIndex("dbo.sms_channel_allot", new[] { "sort_batch_id" });
            AlterColumn("dbo.wms_sorting_line", "sorting_line_type", c => c.String(nullable: false, maxLength: 1, fixedLength: true));
            DropColumn("dbo.wms_deliver_dist", "deliver_order");
            DropColumn("dbo.wms_sort_order_dispatch", "sort_status");
            DropColumn("dbo.wms_sort_order_dispatch", "deliver_line_no");
            DropColumn("dbo.wms_sort_order_dispatch", "batch_sort_id");
            DropColumn("dbo.wms_sorting_line", "product_type");
            DropTable("dbo.sms_sort_order_allot_master");
            DropTable("dbo.sms_sort_order_allot_detail");
            DropTable("dbo.sms_hand_supply");
            DropTable("dbo.sms_channel");
            DropTable("dbo.sms_channel_allot");
            DropTable("dbo.sms_sort_batch");
        }
    }
}
