using System;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Mappers
{
    public class MapperFactory : IMapperFactory
    {

        public IMapper GetMapper<T>() where T : ISyncObject
        {
            string mapperTypeName = "";
            try
            {
                Type objectType = typeof (T);
                string mappernamespace = typeof (SupporterMapper).Namespace;
                mapperTypeName = mappernamespace + "." + objectType.Name + "Mapper";
                Type mapperType = Type.GetType(mapperTypeName);
                return Activator.CreateInstance(mapperType) as IMapper;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Fail to find mapper:" + mapperTypeName, ex);
            }
        }

    }
}