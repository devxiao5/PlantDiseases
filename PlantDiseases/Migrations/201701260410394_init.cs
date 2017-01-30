namespace PlantDiseases.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HtmlPages",
                c => new
                    {
                        DetailId = c.Int(nullable: false, identity: true),
                        Url = c.String(maxLength: 500, storeType: "nvarchar"),
                        Title = c.String(maxLength: 300, storeType: "nvarchar"),
                        Subject = c.String(maxLength: 300, storeType: "nvarchar"),
                        Description = c.String(maxLength: 500, storeType: "nvarchar"),
                        Content = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.DetailId);
            
            CreateTable(
                "dbo.TargetLinks",
                c => new
                    {
                        TargetLinkId = c.Int(nullable: false, identity: true),
                        Url = c.String(maxLength: 500, storeType: "nvarchar"),
                        Done = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TargetLinkId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TargetLinks");
            DropTable("dbo.HtmlPages");
        }
    }
}
