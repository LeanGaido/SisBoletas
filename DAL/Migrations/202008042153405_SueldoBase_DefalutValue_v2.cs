namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SueldoBase_DefalutValue_v2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tblDetallesDeclaracionJurada", "SueldoBase", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tblDetallesDeclaracionJurada", "SueldoBase", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
