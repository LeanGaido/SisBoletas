namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tblParametros_Cambios_tblBoletasDeAportestblBoletasDeAportesEspeciales : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblParametrosGenerales",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.tblBoletaAportes", "TotalSueldos", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.tblBoletaAportes", "Aportes", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.tblBoletaAportes", "TotalSueldosAfiliados", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.tblBoletaAportes", "AportesAfiliados", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.tblBoletasAportesEspeciales", "TotalSueldos", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.tblBoletasAportesEspeciales", "Aportes", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.tblBoletasAportesEspeciales", "TotalSueldosAfiliados", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.tblBoletasAportesEspeciales", "AportesAfiliados", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.tblBoletaAportes", "TotalSueldos2");
            DropColumn("dbo.tblBoletaAportes", "Aportes2");
            DropColumn("dbo.tblBoletaAportes", "TotalSueldos5");
            DropColumn("dbo.tblBoletaAportes", "Aportes5");
            DropColumn("dbo.tblBoletasAportesEspeciales", "TotalSueldos2");
            DropColumn("dbo.tblBoletasAportesEspeciales", "Aportes2");
            DropColumn("dbo.tblBoletasAportesEspeciales", "TotalSueldos5");
            DropColumn("dbo.tblBoletasAportesEspeciales", "Aportes5");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tblBoletasAportesEspeciales", "Aportes5", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.tblBoletasAportesEspeciales", "TotalSueldos5", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.tblBoletasAportesEspeciales", "Aportes2", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.tblBoletasAportesEspeciales", "TotalSueldos2", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.tblBoletaAportes", "Aportes5", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.tblBoletaAportes", "TotalSueldos5", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.tblBoletaAportes", "Aportes2", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.tblBoletaAportes", "TotalSueldos2", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.tblBoletasAportesEspeciales", "AportesAfiliados");
            DropColumn("dbo.tblBoletasAportesEspeciales", "TotalSueldosAfiliados");
            DropColumn("dbo.tblBoletasAportesEspeciales", "Aportes");
            DropColumn("dbo.tblBoletasAportesEspeciales", "TotalSueldos");
            DropColumn("dbo.tblBoletaAportes", "AportesAfiliados");
            DropColumn("dbo.tblBoletaAportes", "TotalSueldosAfiliados");
            DropColumn("dbo.tblBoletaAportes", "Aportes");
            DropColumn("dbo.tblBoletaAportes", "TotalSueldos");
            DropTable("dbo.tblParametrosGenerales");
        }
    }
}
