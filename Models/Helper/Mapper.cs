using AutoMapper;

namespace TestSignalR.Models.Helper
{
    public static class Mapper
    {
        public static TDto Map<TModel, TDto>(TModel model)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TModel, TDto>());
            var mapper = config.CreateMapper();
            return mapper.Map<TDto>(model);
        }
    }
}
