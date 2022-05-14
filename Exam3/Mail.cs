using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Exam3
{
    internal class Mail
    {
        public static async Task SendEmailAsync(string massage)
        {
            MailAddress from = new MailAddress("zakirtest2@gmail.com", "Zakir");
            MailAddress to = new MailAddress("zakir_ur.99@mail.ru");
            MailMessage m = new MailMessage(from, to);
            m.Subject = "Тест";
            m.Body = $"{massage}";
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential("zakirtest2@gmail.com", "123456789z+");
            smtp.EnableSsl = true;
            await smtp.SendMailAsync(m);
            Console.WriteLine("Письмо отправлено");
        }
    }
}
