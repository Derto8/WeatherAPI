using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WeatherClient.Classies;
using WeatherSendClient;

namespace WeatherClient.Pages
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Page
    {
        private MainWindow mainWindow;
        private HubConnection HubConnection;
        private Uri UrlAuthorizationServer;
        private Uri UrlRegistrationServer;
        public Authorization(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            UrlAuthorizationServer = new Uri(ConfigurationManager.AppSettings["ServerAuthorizationUrl"]);
            UrlRegistrationServer = new Uri(ConfigurationManager.AppSettings["ServerRegistrationUrl"]);
            this.mainWindow = mainWindow;
            //HubConnection = new HubConnectionBuilder()
            //    .WithUrl(Url)
            //    .WithAutomaticReconnect()
            //    .Build();
        }

        private async void AuthorizationServer(object sender, RoutedEventArgs e)
        {
            if (tbLogin.Text != "")
            {
                if (tbPassword.Text != "")
                {
                    UserData userData = new UserData() { Login = tbLogin.Text, Password = tbPassword.Text };
                    HttpClient httpClient = new HttpClient();
                    string json = JsonSerializer.Serialize(userData);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    using var response = await httpClient.PostAsync(UrlAuthorizationServer.ToString(), content);
                    if (response.StatusCode == HttpStatusCode.NoContent)
                        MessageBox.Show("Проблема с подключением к базе данных.\nПопробуйте позже");
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                        MessageBox.Show("Такого пользователя не существует");
                    if(response.StatusCode == HttpStatusCode.OK)
                    {
                        string responseText = await response.Content.ReadAsStringAsync();
                        JToken jObject = JObject.Parse(responseText);
                        UserToken.AccessToken = jObject["access_token"].ToString();
                        mainWindow.OpenPage(MainWindow.pages.weather);
                    }
                }
                else MessageBox.Show("Enter your password");
            }
            else MessageBox.Show("Enter your login");
        }

        private async void Registration(object sender, RoutedEventArgs e)
        {
            if (tbLogin.Text != "")
            {
                if (tbPassword.Text != "")
                {
                    UserData userData = new UserData() { Login = tbLogin.Text, Password = tbPassword.Text };
                    HttpClient httpClient = new HttpClient();
                    string json = JsonSerializer.Serialize(userData);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    using var response = await httpClient.PostAsync(UrlRegistrationServer.ToString(), content);
                    if (response.StatusCode == HttpStatusCode.NoContent)
                        MessageBox.Show("Проблема с подключением к базе данных.\nПопробуйте позже");
                    if (response.StatusCode == HttpStatusCode.Conflict)
                        MessageBox.Show("Данный пользователь уже существует в бд");
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string responseText = await response.Content.ReadAsStringAsync();
                        JToken jObject = JObject.Parse(responseText);
                        UserToken.AccessToken = jObject["access_token"].ToString();
                        mainWindow.OpenPage(MainWindow.pages.weather);
                    }
                }
                else MessageBox.Show("Enter your password");
            }
            else MessageBox.Show("Enter your login");
        }
    }
}
