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
    /// Логика взаимодействия для UserInfoPage.xaml
    /// </summary>
    public partial class UserInfoPage : Page
    {
        //задается текущий пользователь
        public static Data_base.User _user = App.CurrentUser;
        //задается байтовый массив для изображения профиля
        private byte[] _mainImageData;
        public UserInfoPage()
        {
            InitializeComponent();
            //при инициализации задаются текущие значения для пользователя
            FirstName.Text = _user.First_name;
            LastName.Text = _user.Last_name;
            Surname.Text = _user.Surname;
            Mail.Text = _user.Mail;
            Phone.Text = _user.Phone_number;
            if(_user.Profile_logo != null)
                UserImage.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(_user.Profile_logo);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        //выбор изображения
        private void BtnChooseImageProfile_Click(object sender, RoutedEventArgs e)
        {
            //создается новый OpenFileDialog
            OpenFileDialog ofd = new OpenFileDialog();
            //задается фильтр используемых форматов
            ofd.Filter = "Image | *.png; *.jpg; *.jpeg";
            if(ofd.ShowDialog() == true)
            {
                //текущему изображению придается значение
                _mainImageData = File.ReadAllBytes(ofd.FileName);
                //значение конвертируется в ImageSource
                UserImage.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(_mainImageData);
            }
        }
        //при нажатии кнопки изменения включается редактирование страницы
        private void BtnChangeProfileData_Click(object sender, RoutedEventArgs e)
        {
            //задаются значения для редактирования
            FirstName.IsEnabled = true; 
            LastName.IsEnabled = true;
            Surname.IsEnabled = true;
            Mail.IsEnabled = true;
            Phone.IsEnabled = true;
            BtnSave.Visibility = Visibility.Visible;
            BtnChooseImageProfile.Visibility = Visibility.Visible;
            BtnChangeProfileData.Visibility = Visibility.Collapsed;
        }
        //метод проверки ошибок
        private string CheckErrors()
        {
            //создание строки с ошибками
            var errorBuilder = new StringBuilder();
            //создание регулярного выражения для проверки почты
            Regex mail = new Regex(@"^[-\w.]+@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,4}$");
            //создание регулярного выражения для проверки телефона
            Regex phone = new Regex(@"(\+7\d{10})");
            //создание телефона пользователя
            var phoneFromDB = App.Context.Users.ToList().FirstOrDefault(p => p.Phone_number.ToLower() == Phone.Text.ToLower());
            //условия появления ошибок
            if(string.IsNullOrEmpty(FirstName.Text))
                errorBuilder.AppendLine("Имя обязательно для заполнения");
            if (FirstName.Text.Length > 30)
                errorBuilder.AppendLine("Вероятнее всего ваше имя меньше 30-и символов");
            if(string.IsNullOrEmpty(LastName.Text))
                errorBuilder.AppendLine("Фамилия обязательна для заполнения");
            if (LastName.Text.Length > 50)
                errorBuilder.AppendLine("Вероятнее всего ваша фамилия меньше 50-и символов");
            if(!string.IsNullOrEmpty(Surname.Text))
            {
                if (Surname.Text.Length > 50)
                    errorBuilder.AppendLine("Вероятнее всего ваше отчество меньше 50-и символов");
            }
            if(string.IsNullOrEmpty(Mail.Text))
                errorBuilder.AppendLine("Почта должна быть заполнена");
            if(phoneFromDB != null && phoneFromDB != _user)
                errorBuilder.AppendLine("Такой телефон уже зарегестрирован");
            //создание почты пользователя
            var mailFromDB = App.Context.Users.ToList().FirstOrDefault(p => p.Mail.ToLower() == Mail.Text.ToLower());
            if(mailFromDB != null && mailFromDB != _user)
                errorBuilder.AppendLine("Такая почта уже зарегестрирована");
            if (!phone.IsMatch(Phone.Text))
                errorBuilder.AppendLine("Вы указали телефон не верного формата");
            if (!mail.IsMatch(Mail.Text))
                errorBuilder.AppendLine("Вы указали несуществующую почту или почту неправельного формата");
            if (errorBuilder.Length > 0)
            {
                errorBuilder.Insert(0, "Устраните следующие ошибки");
            }
            //вывод строки ошибок
            return errorBuilder.ToString();
        }
        //сохранение данных
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //создание строки с ошибками
                var errorMessage = CheckErrors();
                //проверка ошибок
                if (errorMessage.Length > 0)
                {
                    //вывод при ошибке
                    MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    FirstName.Text = _user.First_name;
                    LastName.Text = _user.Last_name;
                    Surname.Text = _user.Surname;
                    Mail.Text = _user.Mail;
                    Phone.Text = _user.Phone_number;
                }
                else
                {
                    //при отсутвии ошибок открывается редактирование
                    _user.First_name = FirstName.Text;
                    _user.Last_name = LastName.Text;
                    _user.Surname = Surname.Text;
                    _user.Mail = Mail.Text;
                    _user.Phone_number = Phone.Text;
                    if (_user.Profile_logo == null)
                    {
                        _user.Profile_logo = _mainImageData;
                    }
                    //сохранение изменений
                    App.Context.SaveChanges();
                }
                //закрытие для редактирования полей
                FirstName.IsEnabled = false;
                LastName.IsEnabled = false;
                Surname.IsEnabled = false;
                Mail.IsEnabled = false;
                Phone.IsEnabled = false;
                BtnSave.Visibility = Visibility.Collapsed;
                BtnChooseImageProfile.Visibility = Visibility.Collapsed;
                BtnChangeProfileData.Visibility = Visibility.Visible;
            }
            catch(Exception)
            {
            }
        }
    }
}
