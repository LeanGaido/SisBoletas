namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tblEmpresa_IdActividanulleable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.tblEmpresas", "IdActividad", "dbo.tblActividades");
            DropIndex("dbo.tblEmpresas", new[] { "IdActividad" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.tblEmpresas", "IdActividad");
            AddForeignKey("dbo.tblEmpresas", "IdActividad", "dbo.tblActividades", "IdActividad", cascadeDelete: true);
        }
    }
}
