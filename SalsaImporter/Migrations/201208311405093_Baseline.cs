namespace SalsaImporter.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Baseline : DbMigration
    {
        public override void Up()
        {
//            CreateTable(
//               "ObjectTypes",
//               c => new
//               {
//                   ObjectType = c.String(nullable: false),
//               })
//               .PrimaryKey(t => t.ObjectType);
//
//            CreateTable(
//              "SalsaDataTypes",
//              c => new
//              {
//                  DataType = c.String(nullable: false),
//              })
//              .PrimaryKey(t => t.DataType);
//
//            CreateTable(
//             "SyncDirections",
//             c => new
//             {
//                 SyncDirection = c.String(nullable: false),
//             })
//             .PrimaryKey(t => t.SyncDirection);

            CreateTable(
                "SessionContexts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        State = c.String(),
                        StartTime = c.DateTime(),
                        FinishedTime = c.DateTime(),
                        MinimumModifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "JobContexts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        JobName = c.String(),
                        StartTime = c.DateTime(),
                        FinishedTime = c.DateTime(),
                        CurrentRecord = c.Int(nullable: false),
                        SessionContext_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("SessionContexts", t => t.SessionContext_Id)
                .Index(t => t.SessionContext_Id);
            
            CreateTable(
                "SyncEvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ObjectId = c.Int(nullable: false),
                        ObjectType = c.String(),
                        Data = c.String(),
                        EventType = c.String(),
                        TimeStamp = c.DateTime(nullable: false),
                        Error = c.String(),
                        Destination = c.String(),
                        SessionContext_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("SessionContexts", t => t.SessionContext_Id)
                .Index(t => t.SessionContext_Id);

            CreateTable(
                "FieldMappings",
                c => new
                         {
                             Id = c.Int(nullable: false, identity: true),
                             ObjectType = c.String(),
                             SalsaField = c.String(),
                             AftField = c.String(),
                             DataType = c.String(),
                         })
                .PrimaryKey(t => t.Id);
               
            
            CreateTable(
                "SyncConfigs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ObjectType = c.String(),
                        SyncDirection = c.String(),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
              

            
        }
        
        public override void Down()
        {
            DropIndex("SyncEvents", new[] { "SessionContext_Id" });
            DropIndex("JobContexts", new[] { "SessionContext_Id" });
            DropForeignKey("SyncEvents", "SessionContext_Id", "SessionContexts");
            DropForeignKey("JobContexts", "SessionContext_Id", "SessionContexts");
            DropTable("SyncConfigs");
            DropTable("FieldMappings");
            DropTable("SyncEvents");
            DropTable("JobContexts");
            DropTable("SessionContexts");
//            DropTable("ObjectTypes");
//            DropTable("SyncDirections");
//            DropTable("SalsaDataTypes");
        }
    }
}
