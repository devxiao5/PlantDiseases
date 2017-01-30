namespace PlantDiseases.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lengthchange : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.HtmlPages", "Url", c => c.String(maxLength: 700, storeType: "nvarchar"));
            AlterColumn("dbo.HtmlPages", "Title", c => c.String(maxLength: 700, storeType: "nvarchar"));
            AlterColumn("dbo.HtmlPages", "Subject", c => c.String(maxLength: 700, storeType: "nvarchar"));
            AlterColumn("dbo.HtmlPages", "Description", c => c.String(maxLength: 700, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.HtmlPages", "Description", c => c.String(maxLength: 500, storeType: "nvarchar"));
            AlterColumn("dbo.HtmlPages", "Subject", c => c.String(maxLength: 300, storeType: "nvarchar"));
            AlterColumn("dbo.HtmlPages", "Title", c => c.String(maxLength: 300, storeType: "nvarchar"));
            AlterColumn("dbo.HtmlPages", "Url", c => c.String(maxLength: 500, storeType: "nvarchar"));
        }
    }
}
