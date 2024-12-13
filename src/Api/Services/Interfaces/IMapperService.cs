using Google.Protobuf.Collections;

namespace Api.Services.Interfaces
{
    public interface IMapperService
    {
        public TDestination Map<TSource, TDestination>(TSource source);
        public List<TDestination> MapList<TSource, TDestination>(List<TSource> sourceItems);
        public RepeatedField<TDestination> ConvertToRepeatedField<TSource, TDestination>(List<TSource> sourceItems);
    }
}