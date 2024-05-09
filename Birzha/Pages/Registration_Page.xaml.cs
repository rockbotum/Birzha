using ScottPlot.Colormaps;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для Registration_Page.xaml
    /// </summary>
    public partial class Registration_Page : Page
    {
        public Registration_Page()
        {
            InitializeComponent();
        }
        //кнопка регистрации
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            //создание переменной, содержащей ошибки
            var errorMessage = CheckErrors();
            //условие вывода ошибки
            if (errorMessage.Length > 0)
            {
                //вывод ошибки
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                    //создание переменной пользователя БД
                    var user = new Data_base.User
                    {
                        //присвоение параметрам БД введенных значений в поля
                        First_name = tbFirst_Name.Text,
                        Last_name = tbLast_Name.Text,
                        Surname = tbSurname.Text,
                        Mail = tbMail.Text,
                        Password = tbPassword.Password,
                        Special_access = false,
                        Phone_number = tbPhone_Number.Text,
                        User_type = App.Context.User_type.FirstOrDefault(p => p.Type_ID == 2),
                    };
                    //добавление пользователя
                    App.Context.Users.Add(user);
                    //сохранение изменений
                    App.Context.SaveChanges();
                    //вывод при успешной регистрации
                    MessageBox.Show("Вы успешно зарегистрировались");
                    //переход на страницу входа
                    NavigationService.Navigate(new Login_Page());
            }
            
        }
        private string CheckErrors()
        {
            //создание регулярного выражения для проверки почты
            Regex mail = new Regex(@"^[-\w.]+@([A-Za-z0-9][-A-Za-z0-9]+\.)+[A-Za-z]{2,4}$");
            //создание регулярного выражения для проверки телефона
            Regex phone = new Regex(@"(\+7\d{10})");
            //создание строки с ошибками
            var errorBuilder = new StringBuilder();
            //проверка ошибок по полям
            if (string.IsNullOrWhiteSpace(tbMail.Text))
                errorBuilder.AppendLine("Почта обязательна для заполнения;");
            if (tbMail.Text.Length > 255)
                errorBuilder.AppendLine("Ваша почта слишком длинная");
            //задается почта пользователя
            var userMail = App.Context.Users.ToList().FirstOrDefault(p => p.Mail.ToLower() == tbMail.Text.ToLower());
            if (userMail != null)
                errorBuilder.AppendLine("Такой пользователь уже существует");
            if(string.IsNullOrEmpty(tbFirst_Name.Text))
                errorBuilder.AppendLine("Имя обязательно для заполненения;");
            if (tbFirst_Name.Text.Length > 20)
                errorBuilder.AppendLine("Ваше имя вероятнее всего меньше 30-и символов");
            if(string.IsNullOrEmpty(tbLast_Name.Text))
                errorBuilder.AppendLine("Фамилия обязательна для заполнения;");
            if (tbLast_Name.Text.Length > 50)
                errorBuilder.AppendLine("Ваша фамилия вероятнее всего меньше 50-и символов");
            if(!string.IsNullOrEmpty(tbSurname.Text))
            {
                if (tbSurname.Text.Length > 50)
                    errorBuilder.AppendLine("Вероятнее всего ваше отчество меньше 50-и символов");
            }
            //задается телефон пользователя
            var Phone = App.Context.Users.ToList().FirstOrDefault(p => p.Phone_number.ToLower() == tbPhone_Number.ToString().ToLower());
            if (Phone != null)
                errorBuilder.AppendLine("Такой номер телефона уже зарегистрирован;");
            if (string.IsNullOrEmpty(tbPassword.Password))
            {
                errorBuilder.AppendLine("Пароль должен быть заполнен;");
            }
            else
            {
                if (tbPassword.Password.Length < 8 || tbPassword.Password.Length > 20)
                    errorBuilder.AppendLine("Введенный пароль менее 8 символов или более 20;");
            }
            if(string.IsNullOrEmpty(tbPassTwo.Password))
                errorBuilder.AppendLine("Повторите пароль");
            if (tbPassword.Password != tbPassTwo.Password)
                errorBuilder.AppendLine("Вы ввели два разных пароля");
            if (!phone.IsMatch(tbPhone_Number.Text))
                errorBuilder.AppendLine("Вы указали телефон не верного формата");
            if(!mail.IsMatch(tbMail.Text))
                errorBuilder.AppendLine("Вы указали несуществующую почту или почту неправельного формата");
            //добавление ошибок в строку, если они есть
            if (errorBuilder.Length > 0)
            {
                errorBuilder.Insert(0, "Устраните следующие ошибки:\n");
            }
            //вывод строки
            return errorBuilder.ToString();
        }
    }
}
