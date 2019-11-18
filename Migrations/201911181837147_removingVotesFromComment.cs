namespace QaProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removingVotesFromComment : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Comments", "Votes");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Comments", "Votes", c => c.Int(nullable: false));
        }
    }
}
