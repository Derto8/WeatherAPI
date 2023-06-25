# WeatherAPIClient

### Стек технологий проекта:
<img src="https://img.shields.io/badge/ASP.NET WEB API-black?style=for-the-badge&logo=.NET&logoColor=512BD4"/> <img src="https://img.shields.io/badge/WPF-black?style=for-the-badge&logo=.NET&logoColor=512BD4"/> <img src="https://img.shields.io/badge/ASP.NET MVC-black?style=for-the-badge&logo=.NET&logoColor=512BD4"/> <img src="https://img.shields.io/badge/SignalR-black?style=for-the-badge&logo=signal&logoColor=3A76F0"/> <img src="https://img.shields.io/badge/JavaScript-black?style=for-the-badge&logo=javascript&logoColor=F7DF1E"/> <img src="https://img.shields.io/badge/ORM EntityFramework-black?style=for-the-badge&logo=.NET&logoColor=512BD4"/> <img src="https://img.shields.io/badge/MSSQL Server-black?style=for-the-badge&logo=microsoftsqlserver&logoColor=CC2927"/> <img src="https://img.shields.io/badge/JSON-black?style=for-the-badge&logo=json&logoColor=white"/> <img src="https://img.shields.io/badge/AJAX-black?style=for-the-badge&logo=javascript&logoColor=3A76F0"/> <img src="https://img.shields.io/badge/JsonWebToken-black?style=for-the-badge&logo=jsonwebtokens&logoColor=white"/>
<img src="https://img.shields.io/badge/JQuery-black?style=for-the-badge&logo=jquery&logoColor=0769AD"/> 

### О проекте:
- Сервер написан на **WebApi**, с использованием **SignalR**, раз в час парсит данные погоды из **[YandexWeatherAPI](https://yandex.ru/dev/weather/)**, обновляет их в базе данных, по запросу от клиента присылает данные о погоде конкретного города клиенту на **WPF(.NET-клиент)**, либо **ASP.NET MVC (JavaScript-клиент)**. Клиент показывает информацию в удобном виде для пользователя. На сервере реализована авторизация клиента (логин-пароль) при помощи **JWT-токена**, настроена **политика CORS** для клиента на **JavaScript** и присутствует база данных с использованием **ORM EntityFramework** для хранения данных пользователя, и данных о погоде в городах.

## Установка проекта
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

### Разработчик
- [Николай Полозов](https://github.com/Derto8)
