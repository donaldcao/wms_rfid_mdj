namespace THOK.Wms.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_sortorder_sortquantity_column : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.wms_sort_order", "sort_quantity_sum", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AddColumn("dbo.wms_sort_order_detail", "sort_quantity", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            DropColumn("dbo.wms_sort_order_detail", "sort_quantity");
            DropColumn("dbo.wms_sort_order", "sort_quantity_sum");
        }
    }
}
