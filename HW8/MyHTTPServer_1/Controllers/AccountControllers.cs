using MyHTTPServer_1.Configuration;
using MyHTTPServer_1.Attributes;
using MyHTTPServer_1.Servises;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyHTTPServer_1.Models;
using MyORM;

namespace MyHTTPServer_1.Controllers
{
    [Controller("Account")]

    public class AccountControllers
    {

        
        private static readonly List<Account> Accounts = new();
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

        [Get("Add")] //localhost:2323/Account/add?login=""&password=""
        public string Add(string login, string password)
        {

            Accounts.Add(new Account
            {
                Id = (uint)Accounts.Count(),
                Login = login,
                Password = password
            });

            return $"User {login} was added";
        }

        [Get("Delete")] //localhost:2323/Account/delete?id="2" 
        public string Delete(string id)
        {
            var acc = Accounts.FirstOrDefault(x => x.Id == uint.Parse(id));

            if (acc is null)
                return $"User with Id:{id} was not found";

            Accounts.Remove(acc);

            return $"User {acc.Login} was deleted";
        }

        [Get("Update")] //localhost:2323/Account/update?id=""&login=""&password=""
        public string Update(string id, string login, string password)
        {
            var account = Accounts.FirstOrDefault(x => x.Id == int.Parse(id));

            if (account is null)
                return $"User with Id:{id} was not found";

            Accounts.Remove(account);

            account.Login = login;
            account.Password = password;

            Accounts.Add(account);

            return $"User {account.Login} was updated";
        }

        [Get("GetAll")] //localhost:2323/Account/getall
        public List<Account> GetAll() => Accounts;

        [Get("GetById")] //localhost:2323/Account/getbyid?id=""
        public Account GetById(string id)
        {
            return Accounts.FirstOrDefault(x => x.Id == uint.Parse(id)) ?? new Account();
        }
    }
}
