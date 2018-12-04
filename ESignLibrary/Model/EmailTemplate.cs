using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESignLibrary.Model
{
    public class EmailTemplate
    {
        public EmailTemplate(string subject, string messageBody)
        {
            this.Subject = subject;
            this.MessageBody = messageBody;
        }

        public string Subject { get; set; }

        public string MessageBody { get; set; }
    }
}
