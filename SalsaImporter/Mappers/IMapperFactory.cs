namespace SalsaImporter.Mappers
{
    public interface IMapperFactory
    {
        IMapper GetMapper(string objectType);
    }
}