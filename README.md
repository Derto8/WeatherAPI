# WeatherAPIClient

### Стек технологий проекта:
<img src="https://img.shields.io/badge/ASP.NET WEB API-black?style=for-the-badge&logo=.NET&logoColor=512BD4"/> <img src="https://img.shields.io/badge/WPF-black?style=for-the-badge&logo=.NET&logoColor=512BD4"/> <img src="https://img.shields.io/badge/ASP.NET MVC-black?style=for-the-badge&logo=.NET&logoColor=512BD4"/> <img src="https://img.shields.io/badge/SignalR-black?style=for-the-badge&logo=signal&logoColor=3A76F0"/> <img src="https://img.shields.io/badge/JavaScript-black?style=for-the-badge&logo=javascript&logoColor=F7DF1E"/> <img src="https://img.shields.io/badge/ORM EntityFramework-black?style=for-the-badge&logo=.NET&logoColor=512BD4"/> <img src="https://img.shields.io/badge/MSSQL Server-black?style=for-the-badge&logo=microsoftsqlserver&logoColor=CC2927"/> <img src="https://img.shields.io/badge/JSON-black?style=for-the-badge&logo=json&logoColor=white"/> <img src="https://img.shields.io/badge/AJAX-black?style=for-the-badge&logo=javascript&logoColor=3A76F0"/> <img src="https://img.shields.io/badge/JsonWebToken-black?style=for-the-badge&logo=jsonwebtokens&logoColor=white"/>
<img src="https://img.shields.io/badge/JQuery-black?style=for-the-badge&logo=jquery&logoColor=0769AD"/> 

# Содержание
- О проекте
- Установка проекта
- Интерфейсы
- Конкретные реализации
- Примеры обращения к API
- Тесты
- Разработчик

# О проекте:
- Сервер написан на **WebApi**, с использованием **SignalR**, раз в час парсит данные погоды из **[YandexWeatherAPI](https://yandex.ru/dev/weather/)**, обновляет их в базе данных, по запросу от клиента присылает данные о погоде конкретного города клиенту на **WPF(.NET-клиент)**, либо **ASP.NET MVC (JavaScript-клиент)**. Клиент показывает информацию в удобном виде для пользователя. На сервере реализована авторизация клиента (логин-пароль) при помощи **JWT-токена**, настроена **политика CORS** для клиента на **JavaScript** и присутствует база данных с использованием **ORM EntityFramework** для хранения данных пользователя, и данных о погоде в городах.

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

Представляет класс унаследованный от **Hub<INotificationClient>**, реализует логику взаимодействия с **Yandex Weather API**, парсинг данных погоды и осуществляет отправку клиенту, с базовым типом аутентификации на основе **JWT-токена**.

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
# Разработчик
- [Николай Полозов](https://github.com/Derto8)
