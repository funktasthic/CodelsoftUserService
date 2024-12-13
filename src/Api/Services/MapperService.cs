using Api.Services.Interfaces;
using AutoMapper;
using Google.Protobuf.Collections;

namespace Api.Services
{
    public class MapperService : IMapperService
    {
        private readonly IMapper _mapper;

        public MapperService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public RepeatedField<TDestination> ConvertToRepeatedField<TSource, TDestination>(List<TSource> sourceItems)
        {
            var mappedObjects = new RepeatedField<TDestination>();
            mappedObjects.AddRange(sourceItems.Select(x => _mapper.Map<TDestination>(x)));
            return mappedObjects;
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return _mapper.Map<TDestination>(source);
        }

        public List<TDestination> MapList<TSource, TDestination>(List<TSource> sourceItems)
        {
            var mappedObjects = sourceItems.Select(x => _mapper.Map<TDestination>(x)).ToList();
            return mappedObjects;
        }
    }
}