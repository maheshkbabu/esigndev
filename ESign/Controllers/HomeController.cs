using System.Collections.Generic;
using System.Web.Mvc;
using ESignLibrary;
using ESign.Models;
using System.Configuration;
using ESignLibrary.Model;
using MySql.Data.MySqlClient;
using System;
using System.Text;

namespace ESign.Controllers
{
    public class HomeController : Controller
    {

        string mycon = "server=localhost;port=3306;database=esign;user=root;password=sql123";
        private MySql.Data.MySqlClient.MySqlConnection con = new MySql.Data.MySqlClient.MySqlConnection();

        public ActionResult Index()
        {
            var myProfile = new ESignProfile(GetDocuSignCredentials());

            return View(myProfile.ListEnvelopes(10));
        }

        [HttpGet]
        public ActionResult CreateDocument()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateDocument(Person person)
        {
            var documentGenerator = new ESignDocumentGenerator(GetDocuSignCredentials(), GetEmailTemplate(), GetDocuSignTemplate());

            documentGenerator.GenerateDocument(person.FullName, person.Email,0);

            return RedirectToAction("Status");
        }

        [HttpGet]
        public ActionResult Status()
        {
            return View();
        }

        private TemplateFacade GetDocuSignTemplate()
        {
            return new TemplateFacade(
                    ConfigurationManager.AppSettings["TemplateId"],
                    new List<string> { ConfigurationManager.AppSettings["RoleName"] }
                );
        }

        private EmailTemplate GetEmailTemplate()
        {
            return new EmailTemplate(
                    ConfigurationManager.AppSettings["EmailSubjectLine"],
                    ConfigurationManager.AppSettings["EmailMessage"]
                );
        }

        private Credentials GetDocuSignCredentials()
        {
            return new Credentials(
                    ConfigurationManager.AppSettings["UserName"],
                    ConfigurationManager.AppSettings["Password"],
                    ConfigurationManager.AppSettings["IntegratorKey"]
                );
        }

        public ActionResult ClientView()
        {
            return View();
        }

        public ActionResult SubmitClient(ClientViewModel ObjModel)
        {
            if(ModelState.IsValid)
            {
                string mycon;
                mycon = "server=localhost;port=3306;database=esignapp;user=root;password=sql123";
                con = new MySql.Data.MySqlClient.MySqlConnection();
                con.ConnectionString = mycon;
                con.Open();

                string InsertStatement = "Insert into tbl_client (Name, PrimaryContact,Address,Website,Email,Phone,IsActive) values ('" + ObjModel.ClientName + "','" + ObjModel.PrimaryContact + "','" + ObjModel.Address + "','" + ObjModel.WebSite + "','" + ObjModel.Email + "','" + ObjModel.PhoneNumber + "',true)";

                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(InsertStatement, con);
                MySqlDataReader Reader = cmd.ExecuteReader();


                con.Close();
            }

            return RedirectToAction("ClientGrid", "Home");
        }

        public ActionResult ClientGrid()
        {
            string mycon;
            mycon = "server=localhost;port=3306;database=esignapp;user=root;password=sql123";
            con = new MySql.Data.MySqlClient.MySqlConnection();
            con.ConnectionString = mycon;
            con.Open();

            ClientViewModel ObjModel = new ClientViewModel();

            ObjModel.lstClient = new List<ClientViewModel>();

            string lstdata = "select Id,Name, PrimaryContact,Address,Website,Email,Phone,IsActive from tbl_client where IsActive = 1";
            MySql.Data.MySqlClient.MySqlCommand cmd1 = new MySql.Data.MySqlClient.MySqlCommand(lstdata, con);

            MySqlDataReader reader = cmd1.ExecuteReader();

            while (reader.Read())
            {
                ObjModel.lstClient.Add(new ClientViewModel
                {
                    Email = reader["Email"].ToString(),
                    ClientName = reader["Name"].ToString(),
                    PrimaryContact = reader["PrimaryContact"].ToString(),
                    WebSite = reader["Website"].ToString(),
                    ClientID = Convert.ToInt32(reader["ID"].ToString()),
                    Address = reader["Address"].ToString(),
                    PhoneNumber = reader["Phone"].ToString(),
                });
            }

            con.Close();

            return View(ObjModel);

        }


        public ActionResult UserGrid()
        {
            string mycon;
            mycon = "server=localhost;port=3306;database=esignapp;user=root;password=sql123";
            con = new MySql.Data.MySqlClient.MySqlConnection();
            con.ConnectionString = mycon;
            con.Open();

            UserListModel ObjModel = new UserListModel();

            ObjModel.lstUser = new List<UserListModel>();

            string lstdata = "select Id,FirstName, LastName,UserName,PhoneNumber,UserType,Clientid,IsActive from tbl_user where IsActive = 1";
            MySql.Data.MySqlClient.MySqlCommand cmd1 = new MySql.Data.MySqlClient.MySqlCommand(lstdata, con);

            MySqlDataReader reader = cmd1.ExecuteReader();

            while (reader.Read())
            {
                ObjModel.lstUser.Add(new UserListModel
                {
                    UserID = Convert.ToInt32(reader["ID"].ToString()),
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    UserName = reader["UserName"].ToString(),
                    UserType = reader["UserType"].ToString(),
                    ClientID = Convert.ToInt32(reader["Clientid"].ToString()),
                    PhoneNumber = reader["PhoneNumber"].ToString(),
                });
            }

            con.Close();

            return View(ObjModel);

        }

        public ActionResult UserView()
        {

            string mycon;
            mycon = "server=localhost;port=3306;database=esignapp;user=root;password=sql123";
            con = new MySql.Data.MySqlClient.MySqlConnection();
            con.ConnectionString = mycon;
            con.Open();


            UserListModel ObjModel = new UserListModel();

            ObjModel.lstClient = new List<ClientList>();

            string lstdata = "select Id,Name from tbl_client where IsActive = 1";
            MySql.Data.MySqlClient.MySqlCommand cmd1 = new MySql.Data.MySqlClient.MySqlCommand(lstdata, con);

            MySqlDataReader reader = cmd1.ExecuteReader();

            while (reader.Read())
            {
                ObjModel.lstClient.Add(new ClientList
                {
                    ClientName = reader["Name"].ToString(),
                    ClientID = Convert.ToInt32(reader["ID"].ToString())
                });
            }

            ObjModel.ClientList = new SelectList(ObjModel.lstClient, "ClientID", "ClientName");

            con.Close();

            return View(ObjModel);
        }

        public ActionResult SubmitUser(UserListModel ObjModel)
        {
            if (ModelState.IsValid)
            {
                string mycon;
                mycon = "server=localhost;port=3306;database=esignapp;user=root;password=sql123";
                con = new MySql.Data.MySqlClient.MySqlConnection();
                con.ConnectionString = mycon;
                con.Open();

                string TokenKey = RandomString(10);

                string InsertStatement = "Insert into tbl_user (FirstName, Lastname,UserName,Password,PhoneNumber,UserType,Clientid,IsActive) values ('" + ObjModel.FirstName + "','" + ObjModel.LastName + "','" + ObjModel.UserName + "','" + ObjModel.Password + "','" + ObjModel.PhoneNumber + "','" + ObjModel.UserType + "','" + ObjModel.ClientID + "',true)";

                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(InsertStatement, con);
                MySqlDataReader Reader = cmd.ExecuteReader();
                con.Close();
                long UserID = cmd.LastInsertedId;
                con.Open();

                DateTime CurrentDate = DateTime.Now.Date;
                string InsertTokenStatement = "Insert into tbl_tokenmanager (Tokenkey,Userid) values ('" + TokenKey +  "','" + Convert.ToInt32(UserID) + "')";

                MySql.Data.MySqlClient.MySqlCommand cmd1 = new MySql.Data.MySqlClient.MySqlCommand(InsertTokenStatement, con);
                MySqlDataReader Reader1 = cmd1.ExecuteReader();

                con.Close();
            }

            return RedirectToAction("UserGrid", "Home");
        }

        // Generate a random string with a given size  
        public string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }


        public ActionResult ConnectedAppGrid()
        {
            string mycon;
            mycon = "server=localhost;port=3306;database=esignapp;user=root;password=sql123";
            con = new MySql.Data.MySqlClient.MySqlConnection();
            con.ConnectionString = mycon;
            con.Open();

            ConnectedappViewModel ObjModel = new ConnectedappViewModel();

            ObjModel.lstConnectedapp = new List<ConnectedappViewModel>();

            string lstdata = "select Id,AppName, IntegrationKey,userid from tbl_connectedapp";
            MySql.Data.MySqlClient.MySqlCommand cmd1 = new MySql.Data.MySqlClient.MySqlCommand(lstdata, con);

            MySqlDataReader reader = cmd1.ExecuteReader();

            while (reader.Read())
            {
                ObjModel.lstConnectedapp.Add(new ConnectedappViewModel
                {
                    AppID = Convert.ToInt32(reader["Id"].ToString()),
                    AppName = reader["AppName"].ToString(),
                    IntegrationKey = reader["IntegrationKey"].ToString()
                });
            }

            con.Close();

            return View(ObjModel);

        }

        public ActionResult ConnectedappView()
        {

            string mycon;
            mycon = "server=localhost;port=3306;database=esignapp;user=root;password=sql123";
            con = new MySql.Data.MySqlClient.MySqlConnection();
            con.ConnectionString = mycon;
            con.Open();


            ConnectedappViewModel ObjModel = new ConnectedappViewModel();

            ObjModel.UserList = new List<UserList>();

            string lstdata = "select Id,FirstName from tbl_user where IsActive = 1";
            MySql.Data.MySqlClient.MySqlCommand cmd1 = new MySql.Data.MySqlClient.MySqlCommand(lstdata, con);

            MySqlDataReader reader = cmd1.ExecuteReader();

            while (reader.Read())
            {
                ObjModel.UserList.Add(new UserList
                {
                    FirstName = reader["FirstName"].ToString(),
                    Id = Convert.ToInt32(reader["Id"].ToString())
                });
            }

            ObjModel.lstUser = new SelectList(ObjModel.UserList, "Id", "FirstName");

            con.Close();

            return View(ObjModel);
        }

        public ActionResult SubmitConnectedapp(ConnectedappViewModel ObjModel)
        {
            if (ModelState.IsValid)
            {
                string mycon;
                mycon = "server=localhost;port=3306;database=esignapp;user=root;password=sql123";
                con = new MySql.Data.MySqlClient.MySqlConnection();
                con.ConnectionString = mycon;
                con.Open();

                string TokenKey = RandomString(10);

                string InsertStatement = "Insert into tbl_connectedapp (AppName, IntegrationKey,userid ) values ('" + ObjModel.AppName + "','" + TokenKey + "','" + ObjModel.UserID + "')";

                MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(InsertStatement, con);
                MySqlDataReader Reader = cmd.ExecuteReader();
                con.Close();
                long UserID = cmd.LastInsertedId;
                con.Open();

                //DateTime CurrentDate = DateTime.Now.Date;
                //string InsertTokenStatement = "Insert into tbl_tokenmanager (Tokenkey,Userid) values ('" + TokenKey + "','" + Convert.ToInt32(UserID) + "')";

                //MySql.Data.MySqlClient.MySqlCommand cmd1 = new MySql.Data.MySqlClient.MySqlCommand(InsertTokenStatement, con);
                //MySqlDataReader Reader1 = cmd1.ExecuteReader();

                con.Close();
            }

            return RedirectToAction("ConnectedAppGrid", "Home");
        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(LoginModel ObjModel)
        {

            string mycon;
            mycon = "server=localhost;port=3306;database=esignapp;user=root;password=sql123";
            con = new MySql.Data.MySqlClient.MySqlConnection();
            con.ConnectionString = mycon;
            con.Open();

            string ExistUser = "select username, password from tbl_user where username ='" + ObjModel.UserName + "' and password='" + ObjModel.Password + "'";
            MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(ExistUser, con);

            string strResult = string.Empty;
            strResult = (string)cmd.ExecuteScalar();

            if (strResult == null)
            {
                con.Close();
                ModelState.AddModelError("Password", "Invalid Password");
                return View();
            }
            else
            {
                string lstdata = "select ID,username, password from  tbl_user where username='" + ObjModel.UserName + "'";
                cmd = new MySql.Data.MySqlClient.MySqlCommand(lstdata, con);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Session["UserID"] = reader["ID"].ToString();
                }

                con.Close();
                Session["UserName"] = ObjModel.UserName;
                return RedirectToAction("Dashboard", "Home");
            }



        }
    }
}