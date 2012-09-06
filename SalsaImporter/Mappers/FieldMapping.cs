namespace SalsaImporter.Mappers
{
    public class MappingRules
    {
        public static string onlyIfBlank = "onlyIfBlank";
        public static string readOnly = "readOnly";
        public static string aftWins = "aftWins";
        public static string salsaWins = "salsaWins";
    }
    public class FieldMapping
    {
        public int Id { get; set; }
        public string ObjectType { get; set; }
        public string SalsaField { get; set; }
        public string AftField { get; set; }
        public string DataType { get; set; }
        public string MappingRule { get; set; }
    }
}