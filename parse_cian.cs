using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading.Tasks;


namespace WebLoadTest
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            //  Regex regex = new Regex(@"\w*");
            //string test = "<g>hit</g>here ?121/";
            //var temp2 = Regex.Match(test, @"<g.*</g>");
            //Console.Write(temp2.Value);
            List<int> IdsOfObjects = new List<int>();
            WebBrowser wb = new WebBrowser();
            
            string urlAddress = "https://www.cian.ru/cat.php?deal_type=rent&engine_version=2&offer_type=offices&office_type%5B0%5D=1&office_type%5B1%5D=2&region=1";
            
            wb.Navigate(urlAddress);
            string data = GetHtml(urlAddress);
            string[] res = Regex.Split(data, @"href=""https://www.cian.ru/rent/commercial/");

            foreach (var item in res)
            {
                int temp;
                if (int.TryParse(new Regex(@"\D+\w*").Replace(item, ""), out temp))
                    IdsOfObjects.Add(temp);
            }

            foreach (var item in IdsOfObjects.Distinct())
            {
                string temp = GetHtml("https://www.cian.ru/rent/commercial/" + item);
                string ss1 = GetTagByClass(temp, "object_descr_addr");
                Console.WriteLine(GetFromTag(ss1));

                string ss2 = GetTagByClass(temp, "object_descr_price");
                Console.WriteLine(GetFromTag(ss2));

                string ss3 = GetTagByClass(temp, "cf-object-descr-add");
                Console.WriteLine(GetFromTag(ss3));

                string ss4 = GetTagByClass(temp, "cf-comm-offer-detail");
                Console.WriteLine(GetFromTag(ss4));

                //string ss5 = GetTagByClass(temp, "object_descr_text");
                //Console.WriteLine(GetFromTag(ss5));

                Console.WriteLine();
                // string ress = ss.Value;
            }
            //class="object_descr_addr"
            Console.ReadLine();
        }
        static string GetTagByClass(string page, string pageClass)
        {
            string Tag = "/";
            string Temp = Regex.Match(page, @"<\w{1,}.\w*class=""" + pageClass + @""".*</\w{1,}>", RegexOptions.IgnoreCase | RegexOptions.Singleline).Value;
            for (int i = 1; i < Temp.Length; i++)
            {
                if (Temp[i] != ' ')
                    Tag += Temp[i];
                else
                    break;
            }
            return Temp.Substring(0, Temp.IndexOf(Tag));
        }
        static string GetFromTag(string ss)
        {
            bool Inside = false;
            ss = ss.Replace("\n", "");
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < ss.Length; i++)
            {
                if (ss[i] == '<')
                    Inside = true;
                if (ss[i] == '>')
                    Inside = false;


                if (!Inside)
                    result.Append(ss[i]);
            }
            result.Replace(">", "");
            // (&\w{ 1,});
            return Regex.Replace(Regex.Replace(result.ToString(), @"&.{4};", " ").Trim(), @"\s{2,}", " ");
        }

        static string GetHtml(string adress)
        {
            string data = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(adress);
            request.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.52 Safari/537.36";
            


            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
            }
            return data;
        }
    }
}
