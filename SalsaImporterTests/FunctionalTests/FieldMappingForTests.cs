using System.Collections.Generic;
using SalsaImporter.Mappers;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.FunctionalTests
{
    public class FieldMappingForTests
    {
        public static void CreateInDatabase()
        {
            using (var db = new AftDbContext())
            {
                db.Database.ExecuteSqlCommand("delete from FieldMappings");
                db.SaveChanges();

                new List<FieldMapping>
                    {
                        new FieldMapping
                            {
                                ObjectType = "supporter",
                                AftField = "First_Name",
                                SalsaField = "First_Name",
                                DataType = "string",
                                MappingRule = MappingRules.aftWins
                            },
                        new FieldMapping
                            {
                                ObjectType = "supporter",
                                AftField = "Last_Name",
                                SalsaField = "Last_Name",
                                DataType = "string",
                                MappingRule = MappingRules.aftWins
                            },
                        new FieldMapping
                            {
                                ObjectType = "supporter",
                                AftField = "Email",
                                SalsaField = "Email",
                                DataType = "string",
                                MappingRule = MappingRules.aftWins
                            },
                        new FieldMapping
                            {
                                ObjectType = "supporter",
                                AftField = "CustomDateTime0",
                                SalsaField = "CustomDateTime0",
                                DataType = "datetime",
                                MappingRule = MappingRules.aftWins
                            },
                        new FieldMapping
                            {
                                ObjectType = "supporter",
                                AftField = "Chapter_KEY",
                                SalsaField = "chapter_KEY",
                                DataType = "int",
                                MappingRule = MappingRules.aftWins
                            },
                        new FieldMapping
                            {
                                ObjectType = "chapter",
                                AftField = "Name",
                                SalsaField = "Name",
                                DataType = "string",
                                MappingRule = MappingRules.aftWins
                            }
                    }.ForEach(f => db.FieldMappings.Add(f));

                db.SaveChanges();
            }
        }
    }
}