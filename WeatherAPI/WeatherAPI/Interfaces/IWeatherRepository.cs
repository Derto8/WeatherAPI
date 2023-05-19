using WeatherModels;

namespace WeatherAPI.Interfaces
{
    public interface IWeatherRepository<TEntity> where TEntity : class
    {
        void Create(TEntity entity);
        IEnumerable<TEntity> Get();
        void Update(TEntity item);
    }
}
