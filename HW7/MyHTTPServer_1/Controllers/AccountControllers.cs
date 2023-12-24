using MyHTTPServer_1.Configuration;
using MyHTTPServer_1.Attributes;
using MyHTTPServer_1.Servises;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHTTPServer_1.Controllers
{
    [Controller("Account")]
    public class AccountControllers
    {
        [Post("SendToEmail")]
        public static void SendToEmail(string city,
            string address,
            string profession,
            string name,
            string lastname,
            string birthday,
            string phone,
            string social = " = ")
        {
            var config = AppSettingsLoader.Instance().Configuration;
            
            new EmailSenderService(config.MailSender,config.PasswordSender, config.ToEmail, config.SmtpServerHost, config.SmtpServerPort).SendEmailAsync(city, address, profession, name, lastname, birthday, phone, social);
            Console.WriteLine("Email was sent successfully!");
        }
    }
}
