using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WeatherAPI.Authorization
{
    public static class AuthorizationExtensions
    {
        public static SymmetricSecurityKey GetSymmetricSecurityKey(string KEY) => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
