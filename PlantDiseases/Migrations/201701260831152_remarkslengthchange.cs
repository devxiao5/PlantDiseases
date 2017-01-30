namespace PlantDiseases.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remarkslengthchange : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TargetLinks", "Remarks", c => c.String(maxLength: 500, storeType: "nvarchar"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TargetLinks", "Remarks", c => c.String(maxLength: 50, storeType: "nvarchar"));
        }
    }
}
