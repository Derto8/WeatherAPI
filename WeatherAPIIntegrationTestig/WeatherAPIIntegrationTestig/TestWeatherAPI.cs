using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Dynamic;
using WeatherAPI.DataBaseContext;
using WeatherAPI.Hubs;
using WeatherAPI.Interfaces;
using WeatherAPIIntegrationTestig.TestExtensions;
using WeatherModels;

namespace WeatherAPIIntegrationTestig
{
    [TestFixture]
    public class TestWeatherAPI
    {

        [Test]
        public async Task SendWeatherClient_SendRequest_ShouldReturnTrue()
        {
            //Arrange

            WebApplicationFactory<Program> webHost = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    //находим реальное подключение к бд в классе Program
                    var dbContextDescriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<WeatherContext>));

                    //удаляем его
                    services.Remove(dbContextDescriptor);

                    //создаём контекст базы данных для тестов
                    services.AddDbContext<WeatherContext>(options =>
                    {
                        options.UseInMemoryDatabase("weather_test_db");
                    });
                });
            });

            //создаём scope для контекста бд
            WeatherContext dbContext = webHost.Services.CreateScope().ServiceProvider.GetService<WeatherContext>();

            //Act

            //заполняем список рандомными данными
            List<Weather> weathers = DataDBContext.GetWeatherData();


            //заполняем бд
            await dbContext.WeatherTable.AddRangeAsync(weathers);
            await dbContext.SaveChangesAsync();

            //создаем мок конфигурации
            var mockConf = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();

            //создаём экземлряр хаба
            var hub = new WeatherHub(mockConf.Object, dbContext);

            //мок экземляр, с которого вызываем методы на клиенте
            var mockClients = new Mock<IHubCallerClients<INotificationClient>>();
            hub.Clients = mockClients.Object;

            //проверка дошли ли данные до клиента
            bool sendCalled = false;

            dynamic all = new ExpandoObject();
            all.getWeather = new Action<List<Weather>>((weathers) =>
            {
                sendCalled = true;
            });

            mockClients.Setup(m => m.All).Returns(all);
            await hub.SendWeatherClient(weathers);

            //Assert
            Assert.True(sendCalled);
        }
    }
}
