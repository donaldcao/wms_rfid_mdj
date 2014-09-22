namespace THOK.Wms.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_pieces_quantity_and_bar_quantity_for_task : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.wcs_task", "pieces_qutity", c => c.Int(nullable: false));
            AddColumn("dbo.wcs_task", "bar_qutity", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.wcs_task", "bar_qutity");
            DropColumn("dbo.wcs_task", "pieces_qutity");
        }
    }
}
