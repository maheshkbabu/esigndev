using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ESign.Models
{
    public class UserListModel
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public SelectList ClientList { get; set; }
        public List<UserListModel> lstUser { get; set; }
        public List<ClientList> lstClient { get; set; }

        
    }

    public class ClientList
    {
        public int ClientID { get; set; }
        public string ClientName { get; set; }
    }
}