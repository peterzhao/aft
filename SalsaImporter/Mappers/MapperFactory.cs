using System;
using System.Collections.Generic;

namespace SalsaImporter.Mappers
{
    public class MapperFactory : IMapperFactory
    {
        private readonly Dictionary<string, string> _dictionary = new Dictionary<string, string>
                                             {
                                                 {"First_Name", "First_Name"}, 
                                                 {"Last_Name", "Last_Name"},  
                                                 {"Email", "Email"}
                                             };

        private readonly Dictionary<string, Dictionary<string, string>> _mappers;

        public MapperFactory()
        {
            _mappers = new Dictionary<string, Dictionary<string, string>>();
            _mappers.Add("supporter", _dictionary);
        }

        public IMapper GetMapper(string name)
        {
            return new Mapper(name, _mappers[name]);
        }
    }
}