namespace THOK.Wms.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_sms_rename_change : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.wms_sort_order_dispatch", "sort_batch_id", c => c.Int(nullable: false));
            AddColumn("dbo.sms_sort_batch", "no_one_project_batch_no", c => c.Int(nullable: false));
            AddColumn("dbo.sms_sort_batch", "no_one_project_sort_date", c => c.DateTime(nullable: false));
            AddColumn("dbo.sms_channel", "product_code", c => c.String(maxLength: 20));
            AddColumn("dbo.sms_channel", "product_name", c => c.String(maxLength: 50));
            AddColumn("dbo.sms_channel", "is_active", c => c.String(nullable: false, maxLength: 2, fixedLength: true));
            AddColumn("dbo.sms_channel", "update_time", c => c.DateTime(nullable: false));
            DropColumn("dbo.wms_sort_order_dispatch", "batch_sort_id");
            DropColumn("dbo.sms_sort_batch", "no_one_batch_no");
            DropColumn("dbo.sms_sort_batch", "sort_date");
            DropColumn("dbo.sms_channel", "default_product_code");
            DropColumn("dbo.sms_channel", "default_product_name");
            DropColumn("dbo.sms_channel", "status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.sms_channel", "status", c => c.String(nullable: false, maxLength: 2, fixedLength: true));
            AddColumn("dbo.sms_channel", "default_product_name", c => c.String(maxLength: 50));
            AddColumn("dbo.sms_channel", "default_product_code", c => c.String(maxLength: 20));
            AddColumn("dbo.sms_sort_batch", "sort_date", c => c.DateTime(nullable: false));
            AddColumn("dbo.sms_sort_batch", "no_one_batch_no", c => c.Int(nullable: false));
            AddColumn("dbo.wms_sort_order_dispatch", "batch_sort_id", c => c.Int(nullable: false));
            DropColumn("dbo.sms_channel", "update_time");
            DropColumn("dbo.sms_channel", "is_active");
            DropColumn("dbo.sms_channel", "product_name");
            DropColumn("dbo.sms_channel", "product_code");
            DropColumn("dbo.sms_sort_batch", "no_one_project_sort_date");
            DropColumn("dbo.sms_sort_batch", "no_one_project_batch_no");
            DropColumn("dbo.wms_sort_order_dispatch", "sort_batch_id");
        }
    }
}
