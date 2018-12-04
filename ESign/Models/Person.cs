using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESign.Models
{
    public class Person
    {
        public string FullName { get; set; }

        public string Email { get; set; }
    }
    public class userlist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string filename { get; set; }
        public string ApproverEmail { get; set; }
        public string ApproverName { get; set; }
        public string ApproverTitle { get; set; }
        public Boolean EnvelopeStatus { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public DateTime SignedDate { get; set; }
        public string FileName { get; set; }
        public string Files { get; set; }
        public string password { get; set; }
        public string Status { get; set; }
        public int UserID { get; set; }
        public string TokenKey { get; set; }
    }

    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class Dashboard
    {
        public int SendCount { get; set; }
        public int WaitingCount { get; set; }
        public int ExpiringCount { get; set; }
        public int CompletedCount { get; set; }
        public string UserName { get; set; }
        public int UserID { get; set; }
        public List<EnvelopeModel> lstEnvelope { get; set; }
    }

    public class EnvelopeModel
    {       
        public string ApproverEmail { get; set; }       
        public string ApproverName { get; set; }        
        public string ApproverTitle { get; set; }       
        public string SenderEmail { get; set; }       
        public string SenderName { get; set; }      
        public string Status { get; set; }       
        public int ID { get; set; }       
        public string FileName { get; set; }      
        public string FilePath { get; set; }
    }
}