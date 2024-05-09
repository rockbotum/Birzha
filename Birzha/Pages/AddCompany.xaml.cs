using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Birzha.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddCompany.xaml
    /// </summary>
    public partial class AddCompany : Page
    {
        //задается текущая компания
        private Data_base.Company _currentCompany = null;
        //задается текущая акция
        private Data_base.Stock _stock = null;
        //задается байтовый массив для изображения
        private byte[] _mainImageData;
        public AddCompany()
        {
            InitializeComponent();
        }

        //задаются параметры для выбранной компании
        public AddCompany(Data_base.Company company)
        {
            InitializeComponent();
            //задается текущая компания
            _currentCompany = company;
            //задается текущая её акция
            _stock = App.Context.Stocks.FirstOrDefault(p => p.Company == _currentCompany.Company_ID);
            //задаются данные об компании и её акции
            TBoxName.Text = _currentCompany.Company_name;
            OneStockCost.Text = _currentCompany.One_stock_cost.ToString();
            Close.Text = _stock.Close.ToString();
            Open.Text = _stock.Open.ToString();
            Low.Text = _stock.Low.ToString();
            Hight.Text = _stock.Hight.ToString();
            PrevDate.Text = _stock.PrevDate.ToString();
            if (_currentCompany.Company_Logo != null)
                CompanyLogo.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(_currentCompany.Company_Logo);
        }

        //выбор логотипа компании
        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            //создание диалогового окна
            OpenFileDialog ofd = new OpenFileDialog();
            //задание фильтра
            ofd.Filter = "Image | *.png; *.jpg; *.jpeg";
            //условие для отображения
            if(ofd.ShowDialog()==true)
            {
                //считывание байтового массива
                _mainImageData = File.ReadAllBytes(ofd.FileName);
                //загрузка в базу
                CompanyLogo.Source = (ImageSource) new ImageSourceConverter().ConvertFrom(_mainImageData);
            }
        }
        //сохранение данных при изменении
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            //создание строки ошибок
            var errorMessage = CheckErrors();
            //проверка ошибки
            if(errorMessage.Length > 0)
            {
                //вывод при ошибке
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //сохранение новых данных при отсутствии или обновление уже существующих
            else
            {
                //если компании нет
                if (_currentCompany == null)
                {
                    //создание переменной компании
                    var company = new Data_base.Company
                    {
                        //задание её значений
                        Company_name = TBoxName.Text,
                        One_stock_cost = decimal.Parse(OneStockCost.Text),
                        Company_Logo = _mainImageData,
                        All_stocks_quantity = 100
                    };
                    //создание переменной акции
                    var stock = new Data_base.Stock
                    {
                        //задание значений акции
                        Company = company.Company_ID,
                        Open = Convert.ToDecimal(Open.Text),
                        Close = Convert.ToDecimal(Close.Text),
                        Hight = Convert.ToDecimal(Hight.Text),
                        Low = Convert.ToDecimal(Low.Text),
                        PrevDate = Convert.ToDateTime(PrevDate.Text)
                    };
                    //добавление компании
                    App.Context.Companies.Add(company);
                    //добавление акции
                    App.Context.Stocks.Add(stock);
                    //сохранение
                    App.Context.SaveChanges();
                }
                //если компания есть
                else
                {
                    //вывод данных о компании
                    _currentCompany.Company_name = TBoxName.Text;
                    _currentCompany.All_stocks_quantity = 100;
                    _currentCompany.One_stock_cost = decimal.Parse(OneStockCost.Text);
                    if (_currentCompany.Company_Logo == null)
                        _currentCompany.Company_Logo = _mainImageData;
                    _stock.Open = decimal.Parse(Open.Text);
                    _stock.Close = decimal.Parse(Close.Text);
                    _stock.Hight = decimal.Parse(Hight.Text);
                    _stock.Low = decimal.Parse(Low.Text);
                    _stock.PrevDate = DateTime.Parse(PrevDate.Text);
                    App.Context.SaveChanges();
                }
                NavigationService.GoBack();
            }
        }
        //метод для проверки ошибок
        private string CheckErrors()
        {
            Regex Bukva = new Regex(@"\D^,");
            //создание строки с ошибками
            var errorBuilder = new StringBuilder();
            //проверка на ошибки
            if(string.IsNullOrWhiteSpace(TBoxName.Text))
                errorBuilder.AppendLine("Название компании обязательно");
            //создание компании из базы
            var companyFromDB = App.Context.Companies.ToList().FirstOrDefault(p => p.Company_name.ToLower() == TBoxName.Text.ToLower());
            if(companyFromDB != null && companyFromDB != _currentCompany)
                errorBuilder.AppendLine("Такая компания уже существует");
            if (TBoxName.Text.Length > 50)
                errorBuilder.AppendLine("Название вашей компании не должно превышать 50-и символов");
            //создание цены акции
            decimal StockCost = 0;
            if(decimal.TryParse(OneStockCost.Text, out StockCost) == false || StockCost <= 0) 
                errorBuilder.AppendLine("Стоимость акции должна быть положительным числом");
            if(string.IsNullOrWhiteSpace(Open.Text) || string.IsNullOrWhiteSpace(Low.Text) || string.IsNullOrWhiteSpace(Hight.Text) || string.IsNullOrWhiteSpace(Close.Text))
                errorBuilder.AppendLine("Данные поля обязательны для заполнения: Open, Low, High, Close;");
            if (PrevDate.Text.IsNullOrEmpty())
                errorBuilder.AppendLine("Дата не выбрана");
            if(Bukva.IsMatch(Open.Text) || Bukva.IsMatch(Low.Text) || Bukva.IsMatch(Close.Text) || Bukva.IsMatch(Hight.Text))
                errorBuilder.AppendLine("В строки Low, Hoght, Close, Open недопустимо вводит не цифры");
            if (errorBuilder.Length > 0)
            {
                errorBuilder.Insert(0, "Устраните следующие ошибки");
            }
            //вывод строки ошибок
            return errorBuilder.ToString();
        }
    }
}
