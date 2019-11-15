namespace QaProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addvotes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DownVotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        userId = c.String(maxLength: 128),
                        questionId = c.Int(),
                        answerId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Answers", t => t.answerId)
                .ForeignKey("dbo.AspNetUsers", t => t.userId)
                .ForeignKey("dbo.Questions", t => t.questionId)
                .Index(t => t.userId)
                .Index(t => t.questionId)
                .Index(t => t.answerId);
            
            CreateTable(
                "dbo.UpVotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        userId = c.String(maxLength: 128),
                        questionId = c.Int(),
                        answerId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Answers", t => t.answerId)
                .ForeignKey("dbo.Questions", t => t.questionId)
                .ForeignKey("dbo.AspNetUsers", t => t.userId)
                .Index(t => t.userId)
                .Index(t => t.questionId)
                .Index(t => t.answerId);
            
            AddColumn("dbo.AspNetUsers", "Reputation", c => c.Int(nullable: false));
            DropColumn("dbo.Answers", "Votes");
            DropColumn("dbo.Questions", "Votes");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Questions", "Votes", c => c.Int(nullable: false));
            AddColumn("dbo.Answers", "Votes", c => c.Int(nullable: false));
            DropForeignKey("dbo.DownVotes", "questionId", "dbo.Questions");
            DropForeignKey("dbo.UpVotes", "userId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UpVotes", "questionId", "dbo.Questions");
            DropForeignKey("dbo.UpVotes", "answerId", "dbo.Answers");
            DropForeignKey("dbo.DownVotes", "userId", "dbo.AspNetUsers");
            DropForeignKey("dbo.DownVotes", "answerId", "dbo.Answers");
            DropIndex("dbo.UpVotes", new[] { "answerId" });
            DropIndex("dbo.UpVotes", new[] { "questionId" });
            DropIndex("dbo.UpVotes", new[] { "userId" });
            DropIndex("dbo.DownVotes", new[] { "answerId" });
            DropIndex("dbo.DownVotes", new[] { "questionId" });
            DropIndex("dbo.DownVotes", new[] { "userId" });
            DropColumn("dbo.AspNetUsers", "Reputation");
            DropTable("dbo.UpVotes");
            DropTable("dbo.DownVotes");
        }
    }
}
