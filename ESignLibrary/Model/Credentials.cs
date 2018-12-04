using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESignLibrary.Model
{
    public class Credentials
    {
        public Credentials(string username, string password, string integratorKey)
        {
            this.Username = username;
            this.Password = password;
            this.IntegratorKey = integratorKey;
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public string IntegratorKey { get; set; }
    }
}
