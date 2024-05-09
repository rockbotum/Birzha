using Birzha.Data_base;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Birzha
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        //получение данных из базы
        public static Data_base.БД_фондовой_биржиEntities3 Context { get; } = new Data_base.БД_фондовой_биржиEntities3();
        //задается текущтй пользователь
        public static Data_base.User CurrentUser = null;
        //задается текущая цена
        public static decimal currentPrice;
        //задается пердыдущая и следующая дата
        public static DateTime PrevDate;
        public static DateTime NextDate;
        //задаются параметры OHLC
        public static double Low;
        public static double High;
        public static double Open;
        public static double Close;
    }
}   

