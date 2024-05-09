using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Birzha.Data_base
{
    public partial class User
    {
        private string _phone;
        public string Phone
        {
            get => _phone;
            set
            {
                if (App.CurrentUser.Phone_number == _phone) return;
                _phone = App.CurrentUser.Phone_number;
                PhoneMask();
            }
        }
        public int PhoneLength { get; set; }
        public async Task PhoneMask()
        {
            PhoneLength = _phone.Length;
            var newVal = Regex.Replace(Phone, @"[^0-9]", "");
            if (PhoneLength != newVal.Length && !string.IsNullOrEmpty(newVal))
            {
                PhoneLength = newVal.Length;
                Phone = string.Empty;
                    if (newVal.Length <= 1)
                    {
                        Phone = Regex.Replace(newVal, @"(\d{1})", "+$1");
                    }
                    else if (newVal.Length <= 4)
                    {
                        Phone = Regex.Replace(newVal, @"(\d{1})(\d{0,3})", "+$1($2)");
                    }
                    else if (newVal.Length <= 7)
                    {
                        Phone = Regex.Replace(newVal, @"(\d{1})(\d{3})(\d{0,3})", "+$1($2)$3");
                    }
                    else if (newVal.Length <= 9)
                    {
                        Phone = Regex.Replace(newVal, @"(\d{1})(\d{3})(\d{0,3})(\d{0,2})", "+$1($2)$3-$4");
                    }
                    else if (newVal.Length > 9)
                    {
                        Phone = Regex.Replace(newVal, @"(\d{1})(\d{3})(\d{0,3})(\d{0,2})(\d{0,2})", "+$1($2)$3-$4-$5");
                    }
            }
        }
    }
}
