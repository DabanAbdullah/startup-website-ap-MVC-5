namespace lawzand.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removephoto : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "fullname", c => c.String());
            DropColumn("dbo.AspNetUsers", "profilephoto");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "profilephoto", c => c.String());
            DropColumn("dbo.AspNetUsers", "fullname");
        }
    }
}
