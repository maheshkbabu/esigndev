using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ESign.Models
{
    public class ConnectedappViewModel
    {
        public int AppID { get; set; }
        public string AppName { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string IntegrationKey { get; set; }
        public List<ConnectedappViewModel> lstConnectedapp { get; set; }
        public SelectList lstUser { get; set; }
        public List<UserList> UserList { get; set; }
    }

    public class UserList
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
    }
}