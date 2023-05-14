using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
        private string Url;
        public WeatherMainPage(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            JToken jObject = JObject.Parse("config.json");
            Url = jObject["ServerConnection"].ToString();
        }

        private Task InitSignalRConnection()
        {
            HubConnection = new HubConnectionBuilder()
                .WithUrl(Url)
                .Build();

            HubConnection.On<WeatherData>("Send", message => weathersList = message.WeathersList);
            return HubConnection.StartAsync();
        }

        private void Forward(object sender, RoutedEventArgs e)
        {

        }

        private async void ConnectServer(object sender, RoutedEventArgs e)
        {
            await InitSignalRConnection();
            buttonConnect.Foreground = (Brush)bc.ConvertFrom("#00CF00");
            if(weathersList is not null )
            {

            }
            while (true)
            {

            }
        }

        private void FillDataGUI()
        {
            
        }

        private void Back(object sender, RoutedEventArgs e)
        {

        }
    }
}
