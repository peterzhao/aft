using System.Collections.Generic;

namespace SalsaImporter.Mappers
{
    public class MapperFactory : IMapperFactory
    {
         private List<FieldMapping> _mappings = new List<FieldMapping>
                                          {
                                              new FieldMapping{AftField = "First_Name", SalsaField = "First_Name", DataType = "string"},
                                              new FieldMapping{AftField = "Last_Name", SalsaField = "Last_Name", DataType = "string"},
                                              new FieldMapping{AftField = "Email", SalsaField = "Email", DataType = "string"},
                                          };
         private readonly Dictionary<string, List<FieldMapping>> _mappers;

        public MapperFactory()
        {
            _mappers = new Dictionary<string, List<FieldMapping>>();
            _mappers.Add("supporter", _mappings);
        }

        public IMapper GetMapper(string name)
        {
            return new Mapper(name, _mappers[name]);
        }
    }
}