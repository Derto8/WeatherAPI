using WeatherSendClient;

namespace WeatherAPI.Interfaces
{
    public interface IUserRepository
    {
        Task<UserData?> FindUserAuth(UserData userData);
        Task<bool> FindUserReg(UserData userData);
        Task RegistrationUser(UserData userData);
    }
}
