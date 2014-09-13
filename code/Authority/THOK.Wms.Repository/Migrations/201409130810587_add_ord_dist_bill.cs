namespace THOK.Wms.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_ord_dist_bill : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.wms_sort_order", name: "DeliverDate", newName: "deliver_date");
            CreateTable(
                "dbo.wms_ord_dist_bill",
                c => new
                    {
                        dist_bill_id = c.String(nullable: false, maxLength: 50),
                        deliver_line_code = c.String(nullable: false, maxLength: 50),
                        deliver_line_name = c.String(nullable: false, maxLength: 50),
                        deliver_man_code = c.String(nullable: false, maxLength: 50),
                        deliver_man_name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.dist_bill_id);
            
            AlterColumn("dbo.wms_sort_order", "deliver_date", c => c.String(nullable: false, maxLength: 14));
            AlterColumn("dbo.wms_sort_order", "is_active", c => c.String(nullable: false, maxLength: 1));
            AlterColumn("dbo.wms_sort_order", "status", c => c.String(nullable: false, maxLength: 1));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.wms_sort_order", "status", c => c.String(nullable: false, maxLength: 1, fixedLength: true));
            AlterColumn("dbo.wms_sort_order", "is_active", c => c.String(nullable: false, maxLength: 1, fixedLength: true));
            AlterColumn("dbo.wms_sort_order", "deliver_date", c => c.String(nullable: false, maxLength: 14, fixedLength: true));
            DropTable("dbo.wms_ord_dist_bill");
            RenameColumn(table: "dbo.wms_sort_order", name: "deliver_date", newName: "DeliverDate");
        }
    }
}
