# WeatherAPI
### Стек технологий проекта:
<img src="https://img.shields.io/badge/ASP.NET WEB API-black?style=for-the-badge&logo=.NET&logoColor=512BD4"/> <img src="https://img.shields.io/badge/WPF-black?style=for-the-badge&logo=.NET&logoColor=512BD4"/> <img src="https://img.shields.io/badge/ASP.NET MVC-black?style=for-the-badge&logo=.NET&logoColor=512BD4"/> <img src="https://img.shields.io/badge/SignalR-black?style=for-the-badge&logo=signal&logoColor=3A76F0"/> <img src="https://img.shields.io/badge/JavaScript-black?style=for-the-badge&logo=javascript&logoColor=F7DF1E"/> <img src="https://img.shields.io/badge/ORM EntityFramework-black?style=for-the-badge&logo=.NET&logoColor=512BD4"/> <img src="https://img.shields.io/badge/MSSQL Server-black?style=for-the-badge&logo=microsoftsqlserver&logoColor=CC2927"/> <img src="https://img.shields.io/badge/JSON-black?style=for-the-badge&logo=json&logoColor=white"/> <img src="https://img.shields.io/badge/AJAX-black?style=for-the-badge&logo=javascript&logoColor=3A76F0"/> <img src="https://img.shields.io/badge/JsonWebToken-black?style=for-the-badge&logo=jsonwebtokens&logoColor=white"/> <img src="https://img.shields.io/badge/JQuery-black?style=for-the-badge&logo=jquery&logoColor=0769AD"/> <img src="https://img.shields.io/badge/ASP.NET WEB API INTEGRATION TESTS NUNIT-black?style=for-the-badge&logo=.NET&logoColor=512BD4"/>
# Содержание
- О проекте
- Установка проекта
- Интерфейсы
	- INotificationClient
  	- IUserRepository
 	- IWeatherRepository	 	
- Конкретные реализации
	- WeatherHub
 	- UserRepository
  	- WeatherRepository
	- WeatherUpdateService
- Примеры обращения к API
  	- Запрос к API с JavaScript клиента
	- Запрос к API с .NET клиента
- Тесты
	- SendWeatherClient_SendRequest_ShouldReturnTrue 
- Разработчик

# О проекте:

Сервер написан на **WebApi**, с использованием **SignalR**, раз в час парсит данные погоды из **[YandexWeatherAPI](https://yandex.ru/dev/weather/)**, обновляет их в базе данных, по запросу от клиента присылает данные о погоде конкретного города клиенту на **WPF(.NET-клиент)**, либо **ASP.NET MVC (JavaScript-клиент)**. Клиент показывает информацию в удобном виде для пользователя. На сервере реализована авторизация клиента (логин-пароль) при помощи **JWT-токена**, настроена **политика CORS** для клиента на **JavaScript** и присутствует база данных с использованием **ORM EntityFramework** для хранения данных пользователя, и данных о погоде в городах.

# Установка проекта
### Проект WeatherAPI:
В файле `appSettings.json`, в строке прописать подключение к вашей базе данных:
``` json
  "ConnectionStrings": {
    "ConnectionDataBase": "Server=(localdb)\\mssqllocaldb;Database=WeatherDataBase;Trusted_Connection=True;"
  }
```
В строке `YandexKey` написать ваш токен для подключения к YandexAPI, получить его можно [здесь](https://yandex.ru/dev/weather/doc/dg/concepts/about.html) 
```json
  "YandexKey": "ваш токен"
```

В строке `AuthOptions` прописать:
```json
  "AuthOptions": {
    "ISSUER": "Издателя токена",
    "AUDIENCE": "Потребителя ключа",
    "KEY": "Ключ для шифрации"
  }
```

### Проект WeatherClient:
В файле `App.config`, в строке прописать подключение к серверу `WeatherAPI`:
```xml
<configuration>
	<appSettings>
		<add key="GetWeatherUrl" value="Ссылка на подключение к хабу сервера, пример: http://localhost:5213/weather"/>
		<add key="ServerAuthorizationUrl" value="Ссылка на соединение с POST-методом авторизации: http://localhost:5213/login"/>
		<add key="ServerRegistrationUrl" value="Ссылка на соединение с POST-методом регистрации: http://localhost:5213/registration"/>
	</appSettings>
</configuration>
```

### Проект WebClient:
В файле `appSettings.json`, в строке прописать подключение к серверу `WeatherAPI`:
```json
  "ConnectionStrings": {
    "GetWeatherUrl": "Ссылка на подключение к хабу сервера, пример: https://localhost:7175/weather",
    "ServerAuthorizationUrl": "Ссылка на соединение с POST-методом авторизации: https://localhost:7175/login",
    "ServerRegistrationUrl": "Ссылка на соединение с POST-методом регистрации: https://localhost:7175/registration"
  }
```

# Интерфейсы
## Интерфейс INotificationClient
**Пространство имен:** WeatherAPI.Interfaces

Представляет интерфейс для строгой типизиации хаба.

```c#
public interface INotificationClient
```
### Методы
- **Task Send(List<Weather> data)** принимает объект типа: список обобщающий класс **Weather**, используется для отправки данных с сервера на клиент.
## Интерфейс IUserRepository
**Пространство имен:** WeatherAPI.Interfaces

Представляет интерфейс взаимодействия с таблицей **UsersTable** в базе данных.

```c#
public interface IUserRepository
```

### Методы
- **Task<UserData?> FindUserAuth(UserData userData)** - принимает объект типа **UserData**, возвращает объект **UserData**, если пользователь найден в таблице **UsersTable**.
- **Task<bool> FindUserReg(UserData userData)** - принимает объект типа **UserData**, занимается поиском пользователя с таким же логином, возвращает **true/false** в зависимости от результатов поиска.
- **Task RegistrationUser(UserData userData)** - принимает объект типа **UserData**, занимается регистрирует нового пользователя в таблицу **UsersTable**.

## Интерфейс IWeatherRepository
**Пространство имен:** WeatherAPI.Interfaces

Представляет интерфейс взаимодействия с таблицей **WeatherTable** в базе данных.

```c#
public interface IWeatherRepository
```

### Методы
- **Task Create(Weather entity)** - принимает объект типа **UserData**, создаёт новую запись о погоде в таблице **WeatherTable**.
- **Task<List<Weather>> Get(string city)** - принимает объект типа **UserData**, возвращает записи о погоде в конкретном городе.
- **Task Update(Weather weatherDataOld, Weather weatherDataNew)** - принимает старую запись о погоде и новую, обновляет записи в таблице **WeatherTable**.
- **Task<bool> FindCity(string city)** - принимает объект типа **string** (название города), по нему ищет в таблице  **WeatherTable** записи, возвращает **true/false** в зависимости от результатов поиска.
- **Task<List<Weather>> GetAllWeathers()** - возвращает объект типа список объектов класса **Weather**, все записи из таблицы **WeatherTable**.

# Конкретные реализации
## WeatherHub
**Пространство имен:** WeatherAPI.Hubs

Представляет класс унаследованный от **Hub< INotificationClient >**, реализует логику взаимодействия с **Yandex Weather API**, парсинг данных погоды и осуществляет отправку клиенту, с базовым типом аутентификации на основе **JWT-токена**.

```c#
public class WeatherHub : Hub<INotificationClient>
```

### Поля и свойства 
- **IConfiguration Configuration** - содержит всю конфигурацию проекта.
- **Uri apiUrl** - содержкит ссылку на **Yandex Weather API**.
- **WeatherContext context** - содержит контекст таблицы **WeatherTable**.
- **WeatherRepository weatherRepository** - репозиторий методов для взаимодействия с таблицей **WeatherTable**.
### Методы
- **Task WeatherMethod(string city, string lattitude, string longitude)** - данный метод, может вызвать только авториванный пользователь, имеющий рабочий **JWT-токен**. Метод принимает данные от клиента, определяет, есть ли данные о погоде конкретного города переменной **city** в таблице **WeatherTable**, в случае, если есть, то вызывает метод **Task SendWeatherClient(List<Weather> weathers)**, но если данные о погоде не найдены, то вызывает метод **Task AddWeather(string city, string lattitude, string longitude)**.
- **Task SendWeatherClient(List<Weather> weathers)** - осуществляет отправку данных о погоде клиенту.
- **Task AddWeather(string city, string lattitude, string longitude)** - составляет **POST-запрос** на парсинг данных из **Yandex Weather API**, затем добавляет записи с данными в базу данных, а после отсылает клиенту.
- **Task<List<Weather>> GetWeatherFromDB(string city)** - возвращает записи погоды из базы данных по конкретному городу.
- **Task<List<Weather>> GetWeather(string city, string lattitude, string longitude)** - составляет **POST-запрос** на парсинг данных из **Yandex Weather API**, возвращает список **Weather** с данными о погоде.
- **Task ClientMessage(string message)** - сообщает о подключении и отключении клиента к серверу.
- **void Dispose(bool disposing)** - метод освобождающий свободные/нерабочие ресурсы приложения.

### Вызов класса WeatherHub в классе Program
```c#
app.MapHub<WeatherHub>("/weather");
```

## UserRepository
**Пространство имен:** WeatherAPI.Repository

Представляет класс реализующий интерфейс **IUserRepository**, реализует логику взаимодействия с таблице **UserTable** базы данных.

```c#
public class UserRepository : IUserRepository
```

### Поля и свойства
- **DbContext Context** - контекст базы данных.
- **DbSet<UserData> DBSet** - определяет, к какой таблице базы данных нужно обращаться.
### Методы
- **Task<UserData?> FindUserAuth(UserData userData)** - принимает объект типа **UserData**, возвращает объект **UserData**, если пользователь найден в таблице **UsersTable**.
- **Task<bool> FindUserReg(UserData userData)** - принимает объект типа **UserData**, занимается поиском пользователя с таким же логином, возвращает **true/false** в зависимости от результатов поиска.
- **Task RegistrationUser(UserData userData)** - принимает объект типа **UserData**, занимается регистрирует нового пользователя в таблицу **UsersTable**.

## WeatherRepository
**Пространство имен:** WeatherAPI.Repository

Представляет класс реализующий интерфейс **IWeatherRepository**, реализует логику взаимодействия с таблице **WeatherTable** базы данных.
```c#
public class WeatherRepository : IWeatherRepository
```
### Поля и свойства
- **DbContext Context** - контекст базы данных.
- **DbSet<Weather> DBSet** - определяет, к какой таблице базы данных нужно обращаться.
### Методы
- **Task Create(Weather entity)** - принимает объект типа **UserData**, создаёт новую запись о погоде в таблице **WeatherTable**.
- **Task<List<Weather>> Get(string city)** - принимает объект типа **UserData**, возвращает записи о погоде в конкретном городе.
- **Task Update(Weather weatherDataOld, Weather weatherDataNew)** - принимает старую запись о погоде и новую, обновляет записи в таблице **WeatherTable**.
- **Task<bool> FindCity(string city)** - принимает объект типа **string** (название города), по нему ищет в таблице  **WeatherTable** записи, возвращает **true/false** в зависимости от результатов поиска.
- **Task<List<Weather>> GetAllWeathers()** - возвращает объект типа список объектов класса **Weather**, все записи из таблицы **WeatherTable**.

## WeatherUpdateService
**Пространство имен:** WeatherAPI.Services

Представляет унаследованный класс от абстрактного класса **BackgroundService**, выполняет фоновую задачу: раз в час обновляет данные о погоде во всех городах, имеющихся в базе данных.
```c#
public class WeatherUpdateService : BackgroundService
```
### Поля и свойства
- **IServiceScopeFactory serviceScopeFactory** - фабрика для создания экземпляров **IServiceScope**, необходима для обращения к контексту базы данных.
- **IConfiguration Configuration** - содержит всю конфигурацию проекта.
- **Uri? apiUrl** - содержкит ссылку на **Yandex Weather API**
- **WeatherRepository weatherRepository** - репозиторий методов для взаимодействия с таблицей **WeatherTable**.
### Методы
- **Task ExecuteAsync(CancellationToken stoppingToken)** - метод абстрактного класса **BackgroundService**, раз в час обновляет данные о погоде каждого города, который содержится в базе данных.
- **Task<List<Weather>> GetWeather(string city, string lattitude, string longitude)** - составляет **POST-запрос** на парсинг данных из **Yandex Weather API**, возвращает список **Weather** с данными о погоде.

### Вызов класса WeatherUpdateService в классе Program
```c#
builder.Services.AddHostedService<WeatherUpdateService>();
```

# Примеры обращения к API
## Запрос к API с JavaScript клиента
Пространство имен: WebClient.Views.Home

```javascript

//объвляем переменную подключения к хабу
const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("<yourServerIpAddress>/weather", { accessTokenFactory: () => token }) // для подключения нужен JWT-токен
    .build();

//обработчик получения данных с сервера
hubConnection.on("Send", function (listWeathers) {
// получаем данные в форме json, данные можно отослать на клиентский-asp.net mvc при помощи ajax, 
//либо обработать каким-нибудь другим способом, пример с ajax:
    $.ajax({
	type: "POST",
	url: '@Url.Action("Method", "Controller")',
	contentType: "application/json; charset=utf-8",
	data: JSON.stringify(listWeathers),
	dataType: "json",
    })
});

// соединение пользователя с хабом
hubConnection.start()
    .catch(err => console.error(err.toString()));

//после соединения пользователя с хабом, пример отправки запроса к серверу на получение данных
await hubConnection.invoke("WeatherMethod", data.сity, data.lattitude, data.longitude)
    .catch(error => console.error(error));
```
Пример получения **JWT-токена** на **JavaScript**:

Пространство имен: WebClient.Views.Home

```javascript
let token;
//отправка запроса на сервер, на авторизацию пользователя
const response = await fetch("<yourServerIpAddress>/login", {
     method: "POST",
     headers: { "Accept": "application/json", "Content-Type": "application/json" },
     body: JSON.stringify({
	Login: document.getElementById("Login").value,
	Password: document.getElementById("Password").value
     }),
});

if (response.ok === true) {
    // получаем данные
    const data = await response.json();
    token = data.access_token;
}
```
## Запрос к API с .NET клиента
Пространство имен: WeatherClient.Pages

```c#
//объвляем переменную подключения к хабу
HubConnection HubConnection = new HubConnectionBuilder()
    .WithUrl("<yourServerIpAddress>/weather", options =>
    {
	options.AccessTokenProvider = () => Task.FromResult(UserToken.AccessToken); // регистрация получения JWT-токена
    })
    .WithAutomaticReconnect() //автоматическое переподключение
    .Build();

//обработчик получения данных с сервера
HubConnection.On<List<Weather>>("Send", data => {
    //какая-то работа с данными (List<Weather>)
});

//соединение пользователя с хабом
await HubConnection.StartAsync();

//после соединения пользователя с хабом, пример отправки запроса к серверу на получение данных
await HubConnection.SendAsync("WeatherMethod", city, lattitude, longitude);
```

Пример получения **JWT-токена** на **.NET**:

Пространство имен: WeatherClient.Pages
```c#
string token = "";
UserData userData = new UserData() { Login = "login", Password = "password" };
HttpClient httpClient = new HttpClient();
//сериализируем данные
string json = JsonSerializer.Serialize(userData);
StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
//отправляем запрос на сервер
using var response = await httpClient.PostAsync("<yourServerIpAddress>/login", content);
//получаем ответ от сервера
if(response.StatusCode == HttpStatusCode.OK)
{
    string responseText = await response.Content.ReadAsStringAsync();
    JToken jObject = JObject.Parse(responseText);
    token = jObject["access_token"].ToString(); 
}
```

# Тесты
## Task SendWeatherClient_SendRequest_ShouldReturnTrue
Пространство имен: WeatherAPIIntegrationTestig

Проверяет подключение к серверу.
```c#
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
    //функция getWeather определяется для фиктивного клиента, чтобы ее можно было вызвать из класса WeatherHub
    all.getWeather = new Action<List<Weather>>((weathers) =>
    {
    	// если данные успешно пришли клиенту, то меняем флаг sendCalled на true
	sendCalled = true;
    });
    mockClients.Setup(m => m.All).Returns(all);
    //отправляем данные клиенту
    await hub.SendWeatherClient(weathers);

    //Assert
    // если данные пришли клиенту, то тест считается пройденным
    Assert.True(sendCalled);
}
```
# Разработчик
- [Николай Полозов](https://github.com/Derto8)
