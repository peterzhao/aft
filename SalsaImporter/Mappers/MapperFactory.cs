using System.Linq;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Mappers
{
    public class MapperFactory : IMapperFactory
    {

        public IMapper GetMapper(string objectType)
        {
            using (var db = new AftDbContext())
            {
                return new Mapper(objectType, db.FieldMappings.Where(map => map.ObjectType == objectType).ToList());
            }
        }

    }
}