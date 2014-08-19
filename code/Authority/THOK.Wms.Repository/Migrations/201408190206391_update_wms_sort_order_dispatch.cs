namespace THOK.Wms.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_wms_sort_order_dispatch : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.wms_sort_order_dispatch", "sort_batch_abnormal_id", c => c.Int(nullable: false));
            AddColumn("dbo.wms_sort_order_dispatch", "sort_batch_pieces_id", c => c.Int(nullable: false));
            AddColumn("dbo.wms_sort_order_dispatch", "sort_batch_manual_id", c => c.Int(nullable: false));
            DropColumn("dbo.wms_sort_order_dispatch", "sort_batch_yx_id");
            DropColumn("dbo.wms_sort_order_dispatch", "sort_batch_zj_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.wms_sort_order_dispatch", "sort_batch_zj_id", c => c.Int(nullable: false));
            AddColumn("dbo.wms_sort_order_dispatch", "sort_batch_yx_id", c => c.Int(nullable: false));
            DropColumn("dbo.wms_sort_order_dispatch", "sort_batch_manual_id");
            DropColumn("dbo.wms_sort_order_dispatch", "sort_batch_pieces_id");
            DropColumn("dbo.wms_sort_order_dispatch", "sort_batch_abnormal_id");
        }
    }
}
