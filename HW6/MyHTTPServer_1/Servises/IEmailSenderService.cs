using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHTTPServer_1.Servises
{
    public interface IEmailSenderService
    {
        public Task SendEmailAsync(string city,
        string address,
        string profession,
        string name,
        string lastname,
        string birthday,
        string phone,
        string social);
    }
}
