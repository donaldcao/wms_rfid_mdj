namespace THOK.Wms.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_sort_supply_position : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.sms_supply_position",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        position_name = c.String(nullable: false, maxLength: 50),
                        position_type = c.String(nullable: false, maxLength: 2),
                        product_code = c.String(maxLength: 20),
                        product_name = c.String(maxLength: 50),
                        position_address = c.Int(nullable: false),
                        position_capacity = c.Int(nullable: false),
                        sorting_line_codes = c.String(),
                        target_supply_addresses = c.String(),
                        description = c.String(),
                        is_active = c.String(nullable: false, maxLength: 1),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.sms_supply_position_storage",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        position_id = c.Int(nullable: false),
                        product_code = c.String(nullable: false, maxLength: 20),
                        product_name = c.String(nullable: false, maxLength: 50),
                        quantity = c.Int(nullable: false),
                        wait_quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.sms_supply_position", t => t.position_id)
                .Index(t => t.position_id);
            
            CreateTable(
                "dbo.sms_supply_task",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        supply_id = c.Int(nullable: false),
                        pack_no = c.Int(nullable: false),
                        sorting_line_code = c.String(nullable: false, maxLength: 20),
                        group_no = c.Int(nullable: false),
                        channel_code = c.String(nullable: false, maxLength: 20),
                        channel_name = c.String(nullable: false, maxLength: 100),
                        product_code = c.String(nullable: false, maxLength: 20),
                        product_name = c.String(nullable: false, maxLength: 50),
                        product_barcode = c.String(maxLength: 6),
                        origin_position_address = c.Int(nullable: false),
                        target_supply_address = c.Int(nullable: false),
                        status = c.String(nullable: false, maxLength: 1),
                    })
                .PrimaryKey(t => t.id);
            
            AddColumn("dbo.wms_sort_order_dispatch", "sort_batch_yx_id", c => c.Int(nullable: false));
            AddColumn("dbo.wms_sort_order_dispatch", "sort_batch_zj_id", c => c.Int(nullable: false));
            AlterColumn("dbo.wms_sort_order_detail", "product_name", c => c.String(nullable: false, maxLength: 40));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.sms_supply_position_storage", "position_id", "dbo.sms_supply_position");
            DropIndex("dbo.sms_supply_position_storage", new[] { "position_id" });
            AlterColumn("dbo.wms_sort_order_detail", "product_name", c => c.String(nullable: false, maxLength: 40, fixedLength: true));
            DropColumn("dbo.wms_sort_order_dispatch", "sort_batch_zj_id");
            DropColumn("dbo.wms_sort_order_dispatch", "sort_batch_yx_id");
            DropTable("dbo.sms_supply_task");
            DropTable("dbo.sms_supply_position_storage");
            DropTable("dbo.sms_supply_position");
        }
    }
}
