using Birzha.Data_base;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace Birzha.Pages
{
    /// <summary>
    /// Логика взаимодействия для CompanyInfoPage.xaml
    /// </summary>
    public partial class CompanyInfoPage : Page
    { 
        //задается переменная для манипуляций с осью дат
        DateTime nextHours;
        //задается таймер
        DispatcherTimer AxesTimer = new DispatcherTimer();

        //задается OHLC (Open, Hight, Low, Close) список для создания финансового графика
        List<OHLC> oHLCs;

        //задаются поля границ
        double Ylimit1;
        double Ylimit2;

        //задается поле интервала
        TimeSpan interval;

        //задаются текущая компания и текущая акция
        private Data_base.Company _company;
        private Data_base.Stock _stock;
        public CompanyInfoPage()
        {
            InitializeComponent();
        }
        public CompanyInfoPage(Data_base.Company company)
        {
            InitializeComponent();
            _company = company;
            //акция принимает значение нажатой компании
            _stock = App.Context.Stocks.FirstOrDefault(p => p.Company == company.Company_ID);
            //вызываеися метод отображения
            Info(_company);
        }
        //изначальная загрузка графика
        private void WpfPlot1_Loaded(object sender, RoutedEventArgs e)
        {
            if (_stock.Company == 1)
            {
                Ylimit1 = App.Low - 1000;
                Ylimit2 = App.Low + 3000;
            }
            else
            {
                Ylimit1 = (double)_stock.Low - 1000;
                Ylimit2 = (double)_stock.Low + 3000;

            }

            try
            {
                GraphFull();
            }
            catch { }
        }
        //метод тика таймера
        private void Time_Tick(object sender, EventArgs e)
        {
            try
            {
                GraphFullTick();
            }
            catch { }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //условие для очистки графика при уходе со страницы
            if (((MainWindow)Application.Current.MainWindow).currentPage != new CompanyInfoPage())
            {
                WpfPlot1.Plot.Clear();
            }
            //задается событие при тике
            AxesTimer.Tick += new EventHandler(Time_Tick);

            //задается интервал тиков
            AxesTimer.Interval = TimeSpan.FromSeconds(2);

            //запускается таймер
            AxesTimer.Start();
        }
        public void Info(Data_base.Company company)
        {

            //задается условие, т.к. для первой компании используется API
            if (company == App.Context.Companies.FirstOrDefault(p => p.Company_ID == 1))
            {

                //очищается график, если до этого была выбрана другая компания
                WpfPlot1.Plot.Clear();

                //обновляется график
                WpfPlot1.Refresh();

                //задается график акций текущей компании
                GraphFull();

                // задаются название, изображение и цена
                if (_company.Company_Logo != null)
                {
                    byte[] data = _company.Company_Logo;
                    MemoryStream byteStream = new MemoryStream(data);
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = byteStream;
                    image.EndInit();
                    CompanyLogo.Source = image;
                }
                NameBlock.Text = _company.Company_name;
                OneStockCost.Text = App.currentPrice.ToString();
            }

            //условие для последующих компаний с статичными данными
            else
            {
                WpfPlot1.Plot.Clear();
                WpfPlot1.Refresh();
                GraphFull();
                if (_company.Company_Logo != null)
                {
                    byte[] data = _company.Company_Logo;
                    MemoryStream byteStream = new MemoryStream(data);
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = byteStream;
                    image.EndInit();
                    CompanyLogo.Source = image;
                }
                NameBlock.Text = _company.Company_name;
                OneStockCost.Text = _company.One_stock_cost.ToString();
            }
        }
        public void GraphFull()
        {
            nextHours = DateTime.Now.AddHours(10);

            //задается условие, т.к. для первой компании используется API 
            if (_stock.Company == 1)
            {
                //задается интервал
                interval = TimeSpan.FromHours(1);

                //создается новый элемент OHLC для графика
                OHLC hLC = new OHLC(App.Open, App.High, App.Low, App.Close, DateTime.Now, interval);

                //элемент заносится в список
                oHLCs = new List<OHLC> { hLC };

                //список выводится в виде финансового графика
                WpfPlot1.Plot.Add.Candlestick(oHLCs);

                //задается нидняя шкала как дата
                WpfPlot1.Plot.Axes.DateTimeTicksBottom();

                //границы для загрузки графика
                WpfPlot1.Plot.RenderManager.RenderStarting += (s, e) =>
                {
                    Tick[] ticks = WpfPlot1.Plot.Axes.Bottom.TickGenerator.Ticks;
                    for (int i = 0; i < ticks.Length; i++)
                    {
                        DateTime dt = DateTime.FromOADate(ticks[i].Position);
                        string lable = $"{dt:dd}.{dt:MM}'{dt:HH} hours";
                        ticks[i] = new Tick(ticks[i].Position, lable);
                    }
                };
                WpfPlot1.Plot.Axes.SetLimitsX(DateTime.Now.AddHours(-2).ToOADate(), nextHours.ToOADate());
                WpfPlot1.Plot.Axes.SetLimitsY(Ylimit1, Ylimit2);
                //обновление графика
                WpfPlot1.Refresh();
            }

            //условие для последующих компаний
            else
            {
                interval = TimeSpan.FromHours(1);
                OHLC hLC = new OHLC((double)_stock.Open, (double)_stock.Hight, (double)_stock.Low, (double)_stock.Close, DateTime.Now, interval);
                oHLCs = new List<OHLC> { hLC };
                WpfPlot1.Plot.Add.Candlestick(oHLCs);
                WpfPlot1.Plot.Axes.DateTimeTicksBottom();
                WpfPlot1.Plot.RenderManager.RenderStarting += (s, e) =>
                {
                    Tick[] ticks = WpfPlot1.Plot.Axes.Bottom.TickGenerator.Ticks;
                    for (int i = 0; i < ticks.Length; i++)
                    {
                        DateTime dt = DateTime.FromOADate(ticks[i].Position);
                        string lable = $"{dt:dd}.{dt:MM}'{dt:HH} hours";
                        ticks[i] = new Tick(ticks[i].Position, lable);
                    }
                };
                WpfPlot1.Plot.Axes.SetLimitsX(DateTime.Now.AddHours(-2).ToOADate(), DateTime.Now.AddHours(10).ToOADate());
                WpfPlot1.Plot.Axes.SetLimitsY(Ylimit1, Ylimit2);

                WpfPlot1.Refresh();
            }
        }
        public void GraphFullTick()
        {
            //задается условие, т.к. для первой компании используется API 
            if (_company == App.Context.Companies.FirstOrDefault(p => p.Company_ID == 1))
            {

                //задается интервал
                interval = TimeSpan.FromHours(1);

                //задается новый элемент OHLC
                OHLC hLC = new OHLC(App.Open, App.High, App.Low, App.Close, DateTime.Now, interval);

                //новый элемент добавляется в список
                oHLCs.Add(hLC);

                //список выводится на график
                WpfPlot1.Plot.Add.Candlestick(oHLCs);

                //график обновляется
                WpfPlot1.Refresh();
            }

            //условие для последующих компаний
            else
            {
                interval = TimeSpan.FromSeconds(1);
                OHLC hLC = new OHLC((double)_stock.Open, (double)_stock.Hight, (double)_stock.Low, (double)_stock.Close, DateTime.Now, interval);
                oHLCs.Add(hLC);
                WpfPlot1.Plot.Add.Candlestick(oHLCs);
                WpfPlot1.Refresh();
            }
        }
        //метод кнопки для расширения графика
        private void Btnplus_Click(object sender, RoutedEventArgs e)
        {
            //добавляет к текущему значению еще 10 часов
            nextHours = nextHours.AddHours(10);
            //генерирует новые границы
            WpfPlot1.Plot.Axes.SetLimitsX(DateTime.Now.AddHours(-2).ToOADate(), nextHours.ToOADate());
            //обновляет график
            WpfPlot1.Refresh();
        }

        private void BtnMinus_Click(object sender, RoutedEventArgs e)
        {

        }

        private void WpfPlot1_MouseEnter(object sender, MouseEventArgs e)
        {
        }
    }
}
