namespace THOK.Wms.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class delete_sortorder_sortquantitysum_column : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.sms_channel", "is_active", c => c.String(nullable: false, maxLength: 1, fixedLength: true));
            DropColumn("dbo.wms_sort_order", "sort_quantity_sum");
        }
        
        public override void Down()
        {
            AddColumn("dbo.wms_sort_order", "sort_quantity_sum", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("dbo.sms_channel", "is_active", c => c.String(nullable: false, maxLength: 2, fixedLength: true));
        }
    }
}
