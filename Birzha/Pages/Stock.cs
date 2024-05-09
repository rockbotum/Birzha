using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Birzha.Pages
{
    //задаются объекты для получения даннхы JSON
    internal class Stock
    {
        object marketdata;
        object securities;

        public object Securities { get => securities; set => securities = value; }
        public object Marketdata { get => marketdata; set => marketdata = value; }
    }
}
