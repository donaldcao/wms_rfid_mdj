namespace THOK.Wms.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class delete_old_sms_table : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.sms_batch_sort", "batch_id", "dbo.sms_batch");
            DropForeignKey("dbo.sms_channel_allot", "batch_sort_id", "dbo.sms_batch_sort");
            DropForeignKey("dbo.sms_led", "led_group_code", "dbo.sms_led");
            DropForeignKey("dbo.sms_channel", "led_code", "dbo.sms_led");
            DropForeignKey("dbo.sms_sort_order_allot_detail", "channel_code", "dbo.sms_channel");
            DropForeignKey("dbo.sms_sort_order_allot_master", "batch_sort_id", "dbo.sms_batch_sort");
            DropForeignKey("dbo.sms_sort_order_allot_detail", "order_master_code", "dbo.sms_sort_order_allot_master");
            DropForeignKey("dbo.sms_sort_supply", "batch_sort_id", "dbo.sms_batch_sort");
            DropForeignKey("dbo.sms_sort_supply", "channel_code", "dbo.sms_channel");
            DropForeignKey("dbo.sms_channel_allot", "channel_code", "dbo.sms_channel");
            DropForeignKey("dbo.sms_deliver_line_allot", "batch_sort_id", "dbo.sms_batch_sort");
            DropIndex("dbo.sms_batch_sort", new[] { "batch_id" });
            DropIndex("dbo.sms_channel_allot", new[] { "batch_sort_id" });
            DropIndex("dbo.sms_channel_allot", new[] { "channel_code" });
            DropIndex("dbo.sms_channel", new[] { "led_code" });
            DropIndex("dbo.sms_led", new[] { "led_group_code" });
            DropIndex("dbo.sms_sort_order_allot_detail", new[] { "order_master_code" });
            DropIndex("dbo.sms_sort_order_allot_detail", new[] { "channel_code" });
            DropIndex("dbo.sms_sort_order_allot_master", new[] { "batch_sort_id" });
            DropIndex("dbo.sms_sort_supply", new[] { "batch_sort_id" });
            DropIndex("dbo.sms_sort_supply", new[] { "channel_code" });
            DropIndex("dbo.sms_deliver_line_allot", new[] { "batch_sort_id" });
            DropTable("dbo.sms_batch");
            DropTable("dbo.sms_batch_sort");
            DropTable("dbo.sms_channel_allot");
            DropTable("dbo.sms_channel");
            DropTable("dbo.sms_led");
            DropTable("dbo.sms_sort_order_allot_detail");
            DropTable("dbo.sms_sort_order_allot_master");
            DropTable("dbo.sms_sort_supply");
            DropTable("dbo.sms_deliver_line_allot");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.sms_deliver_line_allot",
                c => new
                    {
                        deliver_line_allot_code = c.String(nullable: false, maxLength: 50),
                        batch_sort_id = c.Int(nullable: false),
                        deliver_line_code = c.String(nullable: false, maxLength: 50),
                        deliver_quantity = c.Int(nullable: false),
                        status = c.String(nullable: false, maxLength: 2, fixedLength: true),
                    })
                .PrimaryKey(t => t.deliver_line_allot_code);
            
            CreateTable(
                "dbo.sms_sort_supply",
                c => new
                    {
                        sort_supply_code = c.String(nullable: false, maxLength: 50),
                        batch_sort_id = c.Int(nullable: false),
                        supply_id = c.Int(nullable: false),
                        pack_no = c.Int(nullable: false),
                        channel_code = c.String(nullable: false, maxLength: 20),
                        product_code = c.String(maxLength: 20),
                        product_name = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.sort_supply_code);
            
            CreateTable(
                "dbo.sms_sort_order_allot_master",
                c => new
                    {
                        order_master_code = c.String(nullable: false, maxLength: 50),
                        batch_sort_id = c.Int(nullable: false),
                        pack_no = c.Int(nullable: false),
                        order_id = c.String(nullable: false, maxLength: 20),
                        customer_order = c.Int(nullable: false),
                        customer_deliver_order = c.Int(nullable: false),
                        quantity = c.Int(nullable: false),
                        export_no = c.Int(nullable: false),
                        start_time = c.DateTime(nullable: false),
                        finish_time = c.DateTime(nullable: false),
                        status = c.String(nullable: false, maxLength: 2, fixedLength: true),
                    })
                .PrimaryKey(t => t.order_master_code);
            
            CreateTable(
                "dbo.sms_sort_order_allot_detail",
                c => new
                    {
                        order_detail_code = c.String(nullable: false, maxLength: 50),
                        order_master_code = c.String(nullable: false, maxLength: 50),
                        channel_code = c.String(nullable: false, maxLength: 20),
                        product_code = c.String(nullable: false, maxLength: 20),
                        product_name = c.String(nullable: false, maxLength: 50),
                        quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.order_detail_code);
            
            CreateTable(
                "dbo.sms_led",
                c => new
                    {
                        led_code = c.String(nullable: false, maxLength: 20),
                        sorting_line_code = c.String(nullable: false, maxLength: 20),
                        led_name = c.String(nullable: false, maxLength: 100),
                        led_type = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        led_ip = c.String(nullable: false, maxLength: 20),
                        xaxes = c.Int(nullable: false),
                        yaxes = c.Int(nullable: false),
                        width = c.Int(nullable: false),
                        height = c.Int(nullable: false),
                        led_group_code = c.String(nullable: false, maxLength: 20),
                        order_no = c.Int(nullable: false),
                        status = c.String(nullable: false, maxLength: 1, fixedLength: true),
                    })
                .PrimaryKey(t => t.led_code);
            
            CreateTable(
                "dbo.sms_channel",
                c => new
                    {
                        channel_code = c.String(nullable: false, maxLength: 20),
                        sorting_line_code = c.String(nullable: false, maxLength: 20),
                        channel_name = c.String(nullable: false, maxLength: 100),
                        channel_type = c.String(nullable: false, maxLength: 1, fixedLength: true),
                        led_code = c.String(nullable: false, maxLength: 20),
                        default_product_code = c.String(maxLength: 20),
                        remain_quantity = c.Int(nullable: false),
                        middle_quantity = c.Int(nullable: false),
                        max_quantity = c.Int(nullable: false),
                        group_no = c.Int(nullable: false),
                        order_no = c.Int(nullable: false),
                        address = c.Int(nullable: false),
                        cell_code = c.String(maxLength: 20),
                        status = c.String(nullable: false, maxLength: 2, fixedLength: true),
                    })
                .PrimaryKey(t => t.channel_code);
            
            CreateTable(
                "dbo.sms_channel_allot",
                c => new
                    {
                        channel_allot_code = c.String(nullable: false, maxLength: 50),
                        batch_sort_id = c.Int(nullable: false),
                        channel_code = c.String(nullable: false, maxLength: 20),
                        product_code = c.String(maxLength: 20),
                        product_name = c.String(maxLength: 50),
                        in_quantity = c.Int(nullable: false),
                        out_quantity = c.Int(nullable: false),
                        real_quantity = c.Int(nullable: false),
                        remain_quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.channel_allot_code);
            
            CreateTable(
                "dbo.sms_batch_sort",
                c => new
                    {
                        batch_sort_id = c.Int(nullable: false, identity: true),
                        batch_id = c.Int(nullable: false),
                        sorting_line_code = c.String(nullable: false, maxLength: 20),
                        status = c.String(nullable: false, maxLength: 2, fixedLength: true),
                    })
                .PrimaryKey(t => t.batch_sort_id);
            
            CreateTable(
                "dbo.sms_batch",
                c => new
                    {
                        batch_id = c.Int(nullable: false, identity: true),
                        order_date = c.DateTime(nullable: false),
                        batch_no = c.Int(nullable: false),
                        batch_name = c.String(nullable: false, maxLength: 100),
                        operate_date = c.DateTime(nullable: false),
                        operate_person_id = c.Guid(nullable: false),
                        optimize_schedule = c.Int(nullable: false),
                        verify_person_id = c.Guid(),
                        description = c.String(maxLength: 200),
                        project_batch_no = c.Int(nullable: false),
                        status = c.String(nullable: false, maxLength: 2, fixedLength: true),
                    })
                .PrimaryKey(t => t.batch_id);
            
            CreateIndex("dbo.sms_deliver_line_allot", "batch_sort_id");
            CreateIndex("dbo.sms_sort_supply", "channel_code");
            CreateIndex("dbo.sms_sort_supply", "batch_sort_id");
            CreateIndex("dbo.sms_sort_order_allot_master", "batch_sort_id");
            CreateIndex("dbo.sms_sort_order_allot_detail", "channel_code");
            CreateIndex("dbo.sms_sort_order_allot_detail", "order_master_code");
            CreateIndex("dbo.sms_led", "led_group_code");
            CreateIndex("dbo.sms_channel", "led_code");
            CreateIndex("dbo.sms_channel_allot", "channel_code");
            CreateIndex("dbo.sms_channel_allot", "batch_sort_id");
            CreateIndex("dbo.sms_batch_sort", "batch_id");
            AddForeignKey("dbo.sms_deliver_line_allot", "batch_sort_id", "dbo.sms_batch_sort", "batch_sort_id");
            AddForeignKey("dbo.sms_channel_allot", "channel_code", "dbo.sms_channel", "channel_code");
            AddForeignKey("dbo.sms_sort_supply", "channel_code", "dbo.sms_channel", "channel_code");
            AddForeignKey("dbo.sms_sort_supply", "batch_sort_id", "dbo.sms_batch_sort", "batch_sort_id");
            AddForeignKey("dbo.sms_sort_order_allot_detail", "order_master_code", "dbo.sms_sort_order_allot_master", "order_master_code");
            AddForeignKey("dbo.sms_sort_order_allot_master", "batch_sort_id", "dbo.sms_batch_sort", "batch_sort_id");
            AddForeignKey("dbo.sms_sort_order_allot_detail", "channel_code", "dbo.sms_channel", "channel_code");
            AddForeignKey("dbo.sms_channel", "led_code", "dbo.sms_led", "led_code");
            AddForeignKey("dbo.sms_led", "led_group_code", "dbo.sms_led", "led_code");
            AddForeignKey("dbo.sms_channel_allot", "batch_sort_id", "dbo.sms_batch_sort", "batch_sort_id");
            AddForeignKey("dbo.sms_batch_sort", "batch_id", "dbo.sms_batch", "batch_id");
        }
    }
}
