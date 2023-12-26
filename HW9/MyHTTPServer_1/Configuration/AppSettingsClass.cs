using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHTTPServer_1.Configuration
{
    public class AppSettingsClass
    {
        public string Port { get; private set; }
        public string Address { get; private set; }
        public string StaticFilesPath { get; private set; }
        public string MailSender { get; private set; }
        public string PasswordSender { get; private set; }
        public string ToEmail { get; private set; }
        public string SmtpServerHost { get; private set; }
        public ushort SmtpServerPort { get; private set; }

        public AppSettingsClass(string port = "",
            string address = "",
            string staticFilesPath = "",
            string mailSender = "",
            string passwordSender = "",
            string toEmail = "",
            string smtpServerHost = "",
            ushort smtpServerPort = 0)
        {
            Port = port;
            Address = address;
            StaticFilesPath = staticFilesPath;
            MailSender = mailSender;
            PasswordSender = passwordSender;
            ToEmail = toEmail;
            SmtpServerHost = smtpServerHost;
            SmtpServerPort = smtpServerPort;
        }
    }
}
