using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json.Linq;
using Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
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
        private Uri Url;
        private int I = 0;
        public WeatherMainPage(MainWindow mainWindow)
        {
            InitializeComponent();
            Url = new Uri(ConfigurationManager.AppSettings["GetWeatherUrl"]);
            this.mainWindow = mainWindow;
            HubConnection = new HubConnectionBuilder()
                .WithUrl(Url, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(UserToken.AccessToken);
                })
                .WithAutomaticReconnect()
                .Build();
        }


        private void GetData(List<Weather> weatherData)
        {
             this.weathersList = weatherData;
        }

        private void Forward(object sender, RoutedEventArgs e)
        {
            if (weathersList is not null)
            {
                if(I != 24)
                {
                    I += 4;
                    DataToGUI(I);
                }
                else
                    MessageBox.Show("Погоду можно смотреть только на сегодняшнее число и на 6 дней вперёд");
            }
            else
                MessageBox.Show("Вы не подключались к серверу");
        }

        private async void ConnectServer(object sender, RoutedEventArgs e)
        {
            if (tbCity.Text != "")
            {
                try
                {
                    string city = tbCity.Text.ToLower().UpperFirstChar();
                    string? whatCity = CitiesClass.cities.Split("\r\n").Where(c => c.Contains(city)).FirstOrDefault();
                    if (whatCity == null)
                    {
                        MessageBox.Show("Города с таким названием не существует");
                        return;
                    }
                    string[] whatLatLon = whatCity.Split(' ');
                    string lattitude = whatLatLon[2].Remove(whatLatLon[2].Length - 1);
                    string longitude = whatLatLon[3];

                    if(weathersList is not null)
                        weathersList.Clear();
                    HubConnection.On<List<Weather>>("Send", data => GetData(data));
                    await HubConnection.SendAsync("WeatherMethod", city, lattitude, longitude);
                    while (weathersList is null || weathersList.Count == 0)
                    {
                        if (weathersList is not null && weathersList.Count != 0)
                        {
                            break;
                        }
                    }
                    I = 0;
                    DataToGUI(I);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Попробуйте подключиться позже " + HubConnection.State);
                }
            }
            else
                MessageBox.Show("Введите название города");
        }

        private string DefineMonth(string month)
        {
            string[] mas = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            string result = "";

            int m = int.Parse(month);
            for (int i = 0; i <= mas.Length; i++)
            {
                if (m == i)
                {
                    result = mas[i - 1];
                    break;
                }
            }
            return result;
        }

        private async void DataToGUI(int i)
        {
            try
            {
                //date
                Weather w = weathersList[i];
                string? dateWithHours = w.Date.ToString();
                string[]? date = dateWithHours.Split(' ', '.');
                lbDateDay.Content = date[0];
                lbDateMounthYear.Content = $"{DefineMonth(date[1])}\n{date[2]}";

                //morning
                lbMorningT.Content = $"+{w.MinTemperature}°...{w.MaxTemperature}";
                lbMorningText.Content = w.WeatherDescription.UpperFirstChar();
                lbPressureMorning.Content = w.Pressure;
                lbHumidityMorning.Content = w.Humidity;
                lbWindSpeedMorning.Content = $"{w.WindSpeed} {w.WindDir.ToUpper()}";
                lbFeelsMorning.Content = w.FeelsLike;
                await DownloadPicture(w.WeatherImageSource, w.WeatherDescription);
                imgMorning.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @$"\WeatherClient;component\WeatherImages\{w.WeatherDescription}.png"));


                //day
                i = i + 1;
                w = weathersList[i];
                lbDayT.Content = $"+{w.MinTemperature}°...{w.MaxTemperature}";
                lbDayText.Content = w.WeatherDescription.UpperFirstChar();
                lbPressureDay.Content = w.Pressure;
                lbHumidityDay.Content = w.Humidity;
                lbWindSpeedDay.Content = $"{w.WindSpeed} {w.WindDir.ToUpper()}";
                lbFeelsDay.Content = w.FeelsLike;
                await DownloadPicture(w.WeatherImageSource, w.WeatherDescription);
                imgDay.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @$"\WeatherClient;component\WeatherImages\{w.WeatherDescription}.png"));

                //evening
                i = i + 1;
                w = weathersList[i];
                lbEveningT.Content = $"+{w.MinTemperature}°...{w.MaxTemperature}";
                lbEveningText.Content = w.WeatherDescription.UpperFirstChar();
                lbPressureEvening.Content = w.Pressure;
                lbHumidityEvening.Content = w.Humidity;
                lbWindSpeedEvening.Content = $"{w.WindSpeed} {w.WindDir.ToUpper()}";
                lbFeelsEvening.Content = w.FeelsLike;
                await DownloadPicture(w.WeatherImageSource, w.WeatherDescription);
                imgEvening.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @$"\WeatherClient;component\WeatherImages\{w.WeatherDescription}.png"));

                //night
                i = i + 1;
                w = weathersList[i];
                lbNightT.Content = $"+{w.MinTemperature}°...{w.MaxTemperature}";
                lbNightText.Content = w.WeatherDescription.UpperFirstChar();
                lbPressureNight.Content = w.Pressure;
                lbHumidityNight.Content = w.Humidity;
                lbWindSpeedNight.Content = $"{w.WindSpeed} {w.WindDir.ToUpper()}";
                lbFeelsNight.Content = w.FeelsLike;
                await DownloadPicture(w.WeatherImageSource, w.WeatherDescription);
                imgNight.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + @$"\WeatherClient;component\WeatherImages\{w.WeatherDescription}.png"));
            }
            catch(ArgumentOutOfRangeException ex)
            {
                await HubConnection.SendAsync("ClientMessage", $"Произошла ошибка с выводом данных на интрфейс клиента: {ex.Message}");
                await HubConnection.StopAsync();
            }
        }

        private async Task DownloadPicture(Uri? uri, string wDesc)
        {
            string path = @$"WeatherClient;component/WeatherImages/{wDesc}";

            bool fileExists = File.Exists(path + ".png");
            if (fileExists == false)
            {
                byte[] data;
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        using (HttpResponseMessage response = await client.GetAsync(uri))
                        using (HttpContent content = response.Content)
                        {
                            data = await content.ReadAsByteArrayAsync();
                            using (FileStream file = File.Create(path + ".svg"))
                                file.Write(data, 0, data.Length);
                        }

                        SvgDocument svgDocument = SvgDocument.Open(path + ".svg");
                        svgDocument.ShapeRendering = SvgShapeRendering.Auto;

                        Bitmap bmp = svgDocument.Draw(100, 100);
                        bmp.Save(path + ".png", ImageFormat.Png);
                        File.Delete(path + ".svg");
                    }
                    catch (Exception ex)
                    {
                        await HubConnection.SendAsync("ClientMessage", $"Произошла ошибка со скачиваем картинки: {ex.Message}, у пользователя {HubConnection.ConnectionId}");
                        await HubConnection.StopAsync();
                    }
                }
            }
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await HubConnection.StartAsync();
                await HubConnection.SendAsync("ClientMessage", $"Подключился пользователь {HubConnection.ConnectionId}");

            }
            catch
            {
                MessageBox.Show("Невозможно подключиться к серверу");
            }
        }

        private async void Window_Closing(object sender, RoutedEventArgs e)
        {
            await HubConnection.InvokeAsync("ClientMessage", $"Пользователь {HubConnection.ConnectionId} отключился выходит из чата");
            await HubConnection.StopAsync();
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            if (weathersList is not null)
            {
                if (I != 0)
                {
                    I -= 4;
                    DataToGUI(I);
                }
                else
                    MessageBox.Show("Погоду можно смотреть только на сегодняшнее число и на 6 дней вперёд");
            }
            else
                MessageBox.Show("Вы не подключились к серверу!");
        }
    }
}
