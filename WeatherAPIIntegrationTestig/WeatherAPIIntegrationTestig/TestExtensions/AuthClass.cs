using Flurl.Http;

namespace WeatherAPIIntegrationTestig.TestExtensions
{
    internal static class AuthClass
    {

        public static async Task<string> AuthUser()
        {
            var data = new
            {
                Login = "user",
                Password = "user"
            };

            var response = await "https://localhost:7175/login".PostJsonAsync(data).ReceiveString();
            return response;
        }

        public static async Task<string> IncorrectAuthUser()
        {
            var data = new
            {
                Login = "unknown",
                Password = "unknown"
            };

            var response = await "https://localhost:7175/login".PostJsonAsync(data).ReceiveString();
            return response;
        }
    }
}
