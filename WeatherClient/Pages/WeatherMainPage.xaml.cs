using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
using WeatherModels;
using WeatherSendClient;

namespace WeatherClient.Pages
{
    /// <summary>
    /// Логика взаимодействия для WeatherMainPage.xaml
    /// </summary>
    public partial class WeatherMainPage : Page
    {
        private MainWindow mainWindow;
        private BrushConverter bc = new BrushConverter();
        private HubConnection? HubConnection;
        private List<Weather> weathersList;
        private string Url = "http://localhost:5213/weather";
        public WeatherMainPage(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            HubConnection = new HubConnectionBuilder()
                .WithUrl(Url)
                .Build();
        }


        private void GetData(List<Weather> weatherData)
        {
            this.weathersList = weatherData;
        }

        private async void Forward(object sender, RoutedEventArgs e)
        {

        }

        private async void ConnectServer(object sender, RoutedEventArgs e)
        {
            HubConnection.On<List<Weather>>("Send", data => GetData(data));
            await HubConnection.SendAsync("SendWeatherMessage");
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await HubConnection.StartAsync();
                await HubConnection.SendAsync("ClientMessage", $"Подключился пользователь {HubConnection.ConnectionId}");

            }
            catch (Exception ex)
            {
                await HubConnection.SendAsync("ClientMessage", $"Пользователь не смог подключиться: {ex.Message}");
            }
        }

        private async void Window_Closing(object sender, RoutedEventArgs e)
        {
            await HubConnection.InvokeAsync("ClientMessage", $"Пользователь {HubConnection.ConnectionId} отключился выходит из чата");
            await HubConnection.StopAsync();
        }

        private void Back(object sender, RoutedEventArgs e)
        {

        }
    }
}
