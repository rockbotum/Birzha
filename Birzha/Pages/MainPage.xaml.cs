using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ScottPlot;
using ScottPlot.Colormaps;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Runtime.Remoting.Channels;
using System.Data.SqlClient;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Windows.Threading;
using System.Printing;
using System.Diagnostics.Eventing.Reader;
using System.Xml.Schema;
using System.Data.Entity;
using Birzha.Data_base;

namespace Birzha.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            //после инициализации кнопка профиля становится видимой
            ((MainWindow)Application.Current.MainWindow).BtnProfile.Visibility = Visibility.Visible;

            //задается графический список, куда помещаются компании
            LVCompaniesList.ItemsSource = App.Context.Companies.ToList();

            //задается видимость кнопки добавления компаний для администратора
            if(App.CurrentUser.Special_access == true)
            {
                AddNewCompany.Visibility = Visibility.Visible;
            }
            else
            {
                AddNewCompany.Visibility = Visibility.Collapsed;
            }

            //задаются изначальные значения для сортировок
            ComboSortBy.SelectedIndex = 0;
            ComboStockCost.SelectedIndex = 0;

            //создается и запускается поток
            Thread requestAPI = new Thread(new ThreadStart(((MainWindow)Application.Current.MainWindow).ApiThreading));
            requestAPI.Start();

            //обновляется список
            UpdateList();
        }
        private void WpfPlot1_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Time_Tick(object sender, EventArgs e)
        {
        }


        //кнопка изменения компании
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            //задается текущаяя компания по нажатию
            var currentCompany = (sender as Button).DataContext as Data_base.Company;

            //навигация на страницу изменения компании с данными текущей
            NavigationService.Navigate(new AddCompany(currentCompany));
        }

        //кнопка показа информации о компании
        private void BtnShow_Click(object sender, RoutedEventArgs e)
        {
            //задается текущая компания по нажатию
            var currentCompany = (sender as Button).DataContext as Company;
            //навигация на страницу информации о компании с данными текущей
            NavigationService.Navigate(new CompanyInfoPage(currentCompany));
        }

        //метод загрузки страницы
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //обновление списка
            UpdateList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        //класс для приема данных JSON
        public class arr
        {

            //основные обхекты-массивы в JSON
            object[] columns;
            object[] data;

            //задаются методы записи и считывания данных
            public object[] Columns { get => columns; set => columns = value; }
            public object[] Data { get => data; set => data = value; }
        }

        //задается сортировка по алфавитному и не алфавитному порядкам
        private void ComboSortBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateList();
        }

        //задается поиск по названию
        private void TBoxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateList();
        }

        //задается сортировка по цене
        private void ComboStockCost_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateList();
        }

        //метод обновления списка
        private void UpdateList()
        {

            //задается список компаний
            var companies = App.Context.Companies.ToList();

            //условия для сортировок
            if(ComboSortBy.SelectedIndex == 0)
                companies = companies.OrderBy(p => p.Company_name).ToList();
            else
                companies = companies.OrderByDescending(p => p.Company_name).ToList();

            if(ComboStockCost.SelectedIndex == 1)
                companies = companies.Where(p => p.One_stock_cost >= 0 && p.One_stock_cost < 10000).ToList();
            if (ComboStockCost.SelectedIndex == 2)
                companies = companies.Where(p => p.One_stock_cost >= 10000 && p.One_stock_cost < 50000).ToList();
            if (ComboStockCost.SelectedIndex == 3)
                companies = companies.Where(p => p.One_stock_cost >= 50000 && p.One_stock_cost < 100000).ToList();
            if (ComboStockCost.SelectedIndex == 4)
                companies = companies.Where(p => p.One_stock_cost >= 100000 && p.One_stock_cost < 200000).ToList();
            if (ComboStockCost.SelectedIndex == 5)
                companies = companies.Where(p => p.One_stock_cost >= 200000).ToList();

            companies = companies.Where(p => p.Company_name.ToLower().Contains(TBoxName.Text.ToLower())).ToList();

            LVCompaniesList.ItemsSource = companies;
        }

        //метод добавления новой компании в список
        private void AddNewCompany_Click(object sender, RoutedEventArgs e)
        {
            //навигация на форму создания новой компании
            NavigationService.Navigate(new AddCompany());
        }

        //метод удаления компании
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
            //создание текущей компании по нажатию
            var currentCompany = (sender as Button).DataContext as Data_base.Company;
            //создание списка акций компании
            if (MessageBox.Show($"Вы уверены, что хотите удалить эту компанию: " + $"{currentCompany.Company_name}?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                //удаление компании из базы
                App.Context.Companies.Remove(currentCompany);  
                //сохрвнение изменений
                App.Context.SaveChanges();
                //обновление списка
                UpdateList();
            } 
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
        }
    }
}
