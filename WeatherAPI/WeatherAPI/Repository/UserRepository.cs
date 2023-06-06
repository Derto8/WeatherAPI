using Microsoft.EntityFrameworkCore;
using WeatherAPI.Interfaces;
using WeatherModels;
using WeatherSendClient;

namespace WeatherAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private DbContext Context;
        private DbSet<UserData> DBSet;
        public UserRepository(DbContext dbContext)
        {
            Context = dbContext;
            DBSet = Context.Set<UserData>();
        }

        public async Task<UserData?> FindUserAuth(UserData userData)
        {
            UserData? user = await DBSet.FirstOrDefaultAsync(c => c.Login == userData.Login && c.Password == userData.Password);
            return user;
        }

        public async Task<bool> FindUserReg(UserData userData)
        {
            UserData? user = await DBSet.FirstOrDefaultAsync(c => c.Login == userData.Login);
            if (user == null)
                return false;
            return true;
        }

        public async Task RegistrationUser(UserData userData)
        {
            UserData user = new UserData()
            {
                Login = userData.Login,
                Password = userData.Password
            };
            await DBSet.AddAsync(user);
            await Context.SaveChangesAsync();
        }
    }
}
