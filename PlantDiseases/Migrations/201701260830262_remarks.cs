namespace PlantDiseases.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remarks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TargetLinks", "Remarks", c => c.String(maxLength: 50, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TargetLinks", "Remarks");
        }
    }
}
