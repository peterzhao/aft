using SalsaImporter.Synchronization;

namespace SalsaImporter.Mappers
{
    public interface IMapperFactory
    {
        IMapper GetMapper<T>() where T : ISyncObject;
    }
}