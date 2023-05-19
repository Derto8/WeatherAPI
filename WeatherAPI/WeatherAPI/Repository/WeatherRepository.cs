using Microsoft.EntityFrameworkCore;
using WeatherAPI.Interfaces;
using WeatherModels;

namespace WeatherAPI.Repository
{
    public class WeatherRepository<TEntity> : IWeatherRepository<TEntity> where TEntity : class
    {
        private DbContext Context;
        private DbSet<TEntity> DBSet;
        public WeatherRepository(DbContext dbContext)
        {
            Context = dbContext;
            DBSet = Context.Set<TEntity>();
        }
        public async void Create(TEntity entity)
        {
            await DBSet.AddAsync(entity);
            await Context.SaveChangesAsync();
        }

        public IEnumerable<TEntity> Get()
        {
            throw new NotImplementedException();
        }

        public async void Update(TEntity item)
        {
            Context.Entry(item).State = EntityState.Modified;
            await Context.SaveChangesAsync();
        }
    }
}
