using Birzha.Pages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
using static Birzha.Pages.MainPage;

namespace Birzha
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //задаются поля: текущая страница и два массива для данных из API
        public JArray jArray;
        public JArray jArray2;
        public Page currentPage;
        public MainWindow()
        {
            InitializeComponent();

            //при инициализации окна открывается страница авторизации
            FrameMain.Navigate(new Pages.Login_Page());

            //задается текущая страница, открытая в окне
            currentPage = ((MainWindow)Application.Current.MainWindow).FrameMain.Content as Page;

    }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAuthorization_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ProfileBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        //кнопка перехода на страницу профиля
        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {

            //переход на страницу профиля пользователя
            FrameMain.NavigationService.Navigate(new Pages.UserInfoPage());
        }

        //кнопка перехода назад
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
                //переход назад
            if (FrameMain.CanGoBack)
            { 
                FrameMain.GoBack();
            }
            else
            {
                BtnProfile.Visibility = Visibility.Collapsed;
            }
        }

        //метод потока для получения API и фиксирование данных из API
        public void ApiThreading()
        {
            try
            {
                StockRequest request = new StockRequest("http://iss.moex.com/iss/engines/stock/markets/shares/boards/TQBR/securities/YNDX.json");
                request.Run();
                Stock stock = JsonConvert.DeserializeObject<Stock>(request.Response);
                var json = JsonConvert.DeserializeObject<arr>(stock.Securities.ToString());
                var json2 = JsonConvert.DeserializeObject<arr>(stock.Marketdata.ToString());

                //присвоение массивам данных из JSON токена
                jArray = (JArray)json.Data[0];
                jArray2 = (JArray)json2.Data[0];

                //присвоение значений из массивов JSON
                App.currentPrice = (decimal)jArray2[36];
                App.PrevDate = (DateTime)jArray[17];
                App.NextDate = App.PrevDate.AddDays(5);
                App.Open = (double)jArray2[9];
                App.High = (double)jArray2[11];
                App.Low = (double)jArray2[10];
                App.Close = (double)jArray2[12];

                //задается приостановление потока
                Thread.Sleep(2000);
            }
            catch
            {
            }
        }
    }
}
