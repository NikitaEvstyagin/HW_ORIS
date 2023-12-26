using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHTTPServer_1.Models
{
    public class Account
    {
        public uint Id { get; init; }
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
