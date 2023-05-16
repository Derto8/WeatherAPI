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
using WeatherClient.Classies;
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
            DataToGUI(0);
        }

        private async void ConnectServer(object sender, RoutedEventArgs e)
        {
            HubConnection.On<List<Weather>>("Send", data => GetData(data));
            await HubConnection.SendAsync("SendWeatherMessage");
        }

        private void DataToGUI(int i)
        {
            //morning
            Weather w = weathersList[i];
            lbMorningT.Content = $"+{w.MinTemperature}°...{w.MaxTemperature}";
            lbMorningText.Content = w.WeatherDescription.UpperFirstChar();
            lbPressureMorning.Content = w.Pressure;
            lbHumidityMorning.Content = w.Humidity;
            lbWindSpeedMorning.Content = w.WindSpeed;
            lbFeelsMorning.Content = w.FeelsLike;

            //day
            i = i + 1;
            w = weathersList[i];
            lbDayT.Content = $"+{w.MinTemperature}°...{w.MaxTemperature}";
            lbDayText.Content = w.WeatherDescription.UpperFirstChar();
            lbPressureDay.Content = w.Pressure;
            lbHumidityDay.Content = w.Humidity;
            lbWindSpeedDay.Content = w.WindSpeed;
            lbFeelsDay.Content = w.FeelsLike;

            //evening
            i = i + 1;
            w = weathersList[i]; 
            lbEveningT.Content = $"+{w.MinTemperature}°...{w.MaxTemperature}";
            lbEveningText.Content = w.WeatherDescription.UpperFirstChar();
            lbPressureEvening.Content = w.Pressure;
            lbHumidityEvening.Content = w.Humidity;
            lbWindSpeedEvening.Content = w.WindSpeed;
            lbFeelsEvening.Content = w.FeelsLike;

            //night
            i = i + 1;
            w = weathersList[i];
            lbNightT.Content = $"+{w.MinTemperature}°...{w.MaxTemperature}";
            lbNightText.Content = w.WeatherDescription.UpperFirstChar();
            lbPressureNight.Content = w.Pressure;
            lbHumidityNight.Content = w.Humidity;
            lbWindSpeedNight.Content = w.WindSpeed;
            lbFeelsNight.Content = w.FeelsLike;

            i = i + 1;
            w = weathersList[i];


        }

        private string GetPicture(string wDesc)
        {
            if (wDesc == "overcast")
                return "https://yastatic.net/weather/i/icons/funky/dark/ovc.svg";
            if (wDesc == "light-rain")
                return "https://yastatic.net/weather/i/icons/funky/dark/ovc_-ra.svg";
            if (wDesc == "")
                return "";
            return "";
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
