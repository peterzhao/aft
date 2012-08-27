using System;

namespace SalsaImporter.Mappers
{
    public class MapperFactory : IMapperFactory
    {
        public IMapper GetMapper(string name)
        {
            try
            {
                string mappernamespace = typeof(SupporterMapper).Namespace;
                string mapperTypeName = mappernamespace + "." + name + "Mapper";
                Type mapperType = Type.GetType(mapperTypeName);
                return Activator.CreateInstance(mapperType) as IMapper;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Fail to find mapper for " + name, ex);
            }
        }
    }
}