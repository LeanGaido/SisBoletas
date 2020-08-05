namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tblParametrosEmpresas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblParametrosEmpresas",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IdEmpresa = c.Int(nullable: false),
                        Key = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.tblCategorias", "Inactiva", c => c.Boolean(nullable: false));
            AddColumn("dbo.tblJornadas", "Inactiva", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tblJornadas", "Inactiva");
            DropColumn("dbo.tblCategorias", "Inactiva");
            DropTable("dbo.tblParametrosEmpresas");
        }
    }
}
