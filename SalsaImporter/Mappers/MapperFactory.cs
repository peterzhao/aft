using System.Linq;
using SalsaImporter.Synchronization;
using System.Collections.Generic;

namespace SalsaImporter.Mappers
{
    public class MapperFactory : IMapperFactory
    {
        Dictionary<string, IMapper> mappers = new Dictionary<string,IMapper>();

        public IMapper GetMapper(string objectType)
        {
            Logger.Trace("Get mapper for " + objectType);
            if (!mappers.Keys.Contains(objectType))
            {
                using (var db = new AftDbContext())
                {
                    mappers[objectType] = new Mapper(objectType, db.FieldMappings.Where(map => map.ObjectType == objectType).ToList());
                }
            }

            return mappers[objectType];
        }

    }
}