namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Presentismo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tblDetallesDeclaracionJurada", "Presentismo", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tblDetallesDeclaracionJurada", "Presentismo");
        }
    }
}
