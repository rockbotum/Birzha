using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Birzha.Pages
{
    //класс для получения данных JSON из API формата http
    internal class StockRequest
    {
        //задается запрос формата http-веб
        HttpWebRequest request;
        //задается адрес страницы
        string address;
        //задается переменная для получения ответа
        public string Response { get; set; }

        //Устанавливает адрес
        public StockRequest(string address)
        {
            this.address = address;
        }
        //метод для запуска
        public void Run()
        {
            //передает созданное значение адреса как класса запроса-веб
            request = (HttpWebRequest)WebRequest.Create(address);
            //задает метод GET для запроса
            request.Method = "GET";

            try
            {
                //возвращает ответ веб-ресурса
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //задается поток чтения данных из запроса
                var stream = response.GetResponseStream();
                //если запрос не пуст,то он читается
                if (stream != null) Response = new StreamReader(stream).ReadToEnd();
            }
            catch (Exception)
            {
            }
        }
    }
}
