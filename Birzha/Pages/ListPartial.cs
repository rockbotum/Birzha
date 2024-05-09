using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Birzha.Data_base
{
    public partial class Company
    {
        //задаются особенности специальных возможностей
        public Visibility AdmiinControlsVisibility
        {
            get
            {
                if(App.CurrentUser.Special_access == true)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }
        //задается получение текущей цены
        public string GetPrice
        {
            get
            {
                App.currentPrice = (decimal)((MainWindow)Application.Current.MainWindow).jArray2[37];
                return App.currentPrice.ToString();
            }
        }
    }
}
