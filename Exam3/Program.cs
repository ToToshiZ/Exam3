using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Exam3
{
    internal class Program
    {
        static List<TableExchangeRate> tableList = new List<TableExchangeRate>();
        //TableExchangeRate table = new TableExchangeRate();
        static void Main(string[] args)
        {
            int num = 0;
            TimerCallback tm = new TimerCallback(ChecXml);
            Timer timer = new Timer(tm, num, 0, 18000);
            SqlAdd();
            UpdateDate();
            Console.ReadLine();
        }

        private static void ChecXml(object obj)
        {
            string chec = "";

                XmlDocument doc = new XmlDocument();
                doc.Load("https://www.nationalbank.kz/rss/rates_all.xml");
                XmlNodeList cl = doc.GetElementsByTagName("item");
                tableList.Clear();
            try
            {
                foreach (XmlNode item in cl)
                {
                    TableExchangeRate table = new TableExchangeRate();

                    table.Title = item["title"].InnerText;
                    table.PubDate = DateTime.Parse(item["pubDate"].InnerText);
                    table.Quant = int.Parse(item["quant"].InnerText);
                    table.Description = double.Parse(item["description"].InnerText.Replace('.', ','));
                    table.Index = item["index"].InnerText;
                    table.Change = double.Parse(item["change"].InnerText.Replace('.', ','));
                    tableList.Add(table);
                    chec = table.Title;
                }
            }
            catch (Exception ex)
            {
                Mail.SendEmailAsync($"{ex.Message}" + $"{chec}").GetAwaiter();
            }
            SqlUpdate();
        }
        private static void SqlUpdate()
        {
            var Lis = tableList;
            try
            {
                foreach (TableExchangeRate elem in Lis)
                {
                    using (Exm3Entities db = new Exm3Entities())
                    {
                        try
                        {
                            TableExchangeRate rate = db.TableExchangeRate.FirstOrDefault(w => w.Title == elem.Title);
                            if (rate.Description != elem.Description)
                            {
                                Mail.SendEmailAsync($"Произошло обновление{elem.Title} Было {rate.Description} Стало{elem.Description} ").GetAwaiter();
                                rate.PubDate = elem.PubDate;
                                rate.Description = elem.Description;
                                db.SaveChanges();
                            }
                        }
                        catch
                        {
                            Console.WriteLine($"Обновлений {elem.Title} нет");
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Mail.SendEmailAsync($"{ex.Message}").GetAwaiter();
            }
        }
        private static void SqlAdd()
        {
            foreach (TableExchangeRate elem in tableList)
            {
                using (Exm3Entities db = new Exm3Entities())
                {
                    db.TableExchangeRate.Add(elem);
                    db.SaveChanges();
                }
            }
        }
        private static void UpdateDate()
        {
            DateTime localDate = DateTime.Today;
            using (Exm3Entities db= new Exm3Entities())
            {
                int count = db.TableExchangeRate.Where(f => f.PubDate == localDate).Count();
                if (count != 0)
                {
                    Console.WriteLine("Сег обнова была");
                }
            }
        }
    }
}
