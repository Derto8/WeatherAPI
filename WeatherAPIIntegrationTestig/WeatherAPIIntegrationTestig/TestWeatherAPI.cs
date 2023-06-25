using Castle.Core.Configuration;
using Flurl.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherAPI.Authorization;
using WeatherAPI.DataBaseContext;
using WeatherAPI.Hubs;
using WeatherAPI.Interfaces;
using WeatherAPIIntegrationTestig.TestExtensions;
using WeatherModels;

namespace WeatherAPIIntegrationTestig
{
    [TestFixture]
    public class TestAuthorization
    {

        [Test]
        public async Task WeatherMethodt_SendRequest_ShouldReturnListWeather()
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

            List<Weather> weathers = DataDBContext.GetWeatherData();


            //заполняем бд рандомными данными
            await dbContext.WeatherTable.AddRangeAsync(weathers);
            await dbContext.SaveChangesAsync();

            //создаем мок конфигурации
            var mockRepo = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();

            //создаём экземлряр хаба
            var hub = new WeatherHub(mockRepo.Object, dbContext);

            //мок экземляр, с которого вызываем методы на клиенте
            var mockClients = new Mock<IHubCallerClients<INotificationClient>>();
            hub.Clients = mockClients.Object;

            bool sendCalled = false;

            dynamic all = new ExpandoObject();
            all.broadcastMessage = new Action<List<Weather>>((weathers) =>
            {
                sendCalled = true;
            });

         //   mockClients.Setup(m => m.All).Returns((ExpandoObject)all);
            await hub.SendWeatherClient(weathers);

            //Assert

            Assert.True(sendCalled);
        }
    }
}
