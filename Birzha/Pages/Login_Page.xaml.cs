using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Birzha.Pages
{
    /// <summary>
    /// Логика взаимодействия для Login_Page.xaml
    /// </summary>
    public partial class Login_Page : Page
    {
        //задается первый вход
        bool firstEnter;
        //задается поле, где будет генерироваться каптча
        string captcha;
        public Login_Page()
        {
            InitializeComponent();
            //задается первый вход при инициализации
            firstEnter = true;
        }
        //переход на страницу регистрации
        private void BtnAuthorization_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Pages.Registration_Page());
        }
        //выполнить вход
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            //задается текущий пользователь
            var currentUser = App.Context.Users.FirstOrDefault(p => p.Mail == tbLogin.Text && p.Password == tbPassword.Password);
            //задается условие для отображения каптчи
            if (firstEnter)
            {
                if (currentUser != null)
                {
                    App.CurrentUser = currentUser;
                    NavigationService.Navigate(new MainPage());
                }
                else
                {
                    MessageBox.Show("Нет пользователя с такими данными.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    //при неудачной авторизации задается не первый вход
                    firstEnter = false;
                    //отображение каптчи
                    Captcha.Visibility = Visibility.Visible;
                    //отображение кнопки обновления каптчи
                    BtnCaptchaUpdate.Visibility = Visibility.Visible;
                    //генерация каптчи
                    CaptchaGenerate();
                }
            }
            else if(!firstEnter && CheckCaptcha() && currentUser != null) 
            {
                App.CurrentUser = currentUser;
                NavigationService.Navigate(new MainPage());
            }
            else
            {
                MessageBox.Show("Пользователь не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                firstEnter= false;
                CaptchaGenerate();
            }
        }
        //метод генерации каптчи
        private void CaptchaGenerate()
        {
            captcha = "";
            string[] element = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", };
            Random rnd = new Random();
            captcha += element[rnd.Next(10, element.Length)] + element[rnd.Next(9)];
            for (int i = 0; i < rnd.Next(4, 9); i++)
            {
                captcha += element[rnd.Next(element.Length)];
            }
            CaptchaTxt.Text = captcha;
        }
        //метод проверки каптчи
        private bool CheckCaptcha()
        {
            if (CaptchaBox.Text == captcha) 
                return true;
            return false;
        }
        //кнопка обновления каптчи
        private void BtnCaptchaUpdate_Click(object sender, RoutedEventArgs e)
        {
            CaptchaGenerate();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //отображение кнопки профиля
            ((MainWindow)System.Windows.Application.Current.MainWindow).BtnProfile.Visibility = Visibility.Collapsed;
        }
    }
}
