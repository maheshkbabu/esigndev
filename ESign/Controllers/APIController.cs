using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Xml;
using ESign.Models;
using ESignLibrary;
using ESignLibrary.Model;
using MySql.Data.MySqlClient;

namespace ESign.Controllers
{
    public class APIController : ApiController
    {
        private MySql.Data.MySqlClient.MySqlConnection con;



        [HttpPost]
        public HttpResponseMessage saveUser(userlist ObjModel)
        {

            string mycon;
            mycon = "server=localhost;port=3306;database=esignapp;user=root;password=sql123";
            con = new MySql.Data.MySqlClient.MySqlConnection();
            con.ConnectionString = mycon;
            con.Open(); 

            string ExistUser = "select id,username, password from tbl_user where username ='" + ObjModel.Name + "' and password='" + ObjModel.password + "'";
            MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(ExistUser, con);

            
            int strResult = (int)cmd.ExecuteScalar();

            if (strResult != 0)
            {

                string ExistToken = "select Tokenkey, Userid from tbl_tokenmanager where Userid ='" + strResult + "'";
                MySql.Data.MySqlClient.MySqlCommand cmd1 = new MySql.Data.MySqlClient.MySqlCommand(ExistToken, con);

                MySqlDataReader reader = cmd1.ExecuteReader();

                string TokenKey = string.Empty;

                while (reader.Read())
                {

                    TokenKey = reader["Tokenkey"].ToString();

                }

                if (TokenKey == ObjModel.TokenKey)
                {
                    con.Close();

                    con.Open();
                    // Read file data
                    FileStream fs = new FileStream(@"C:/Users/ue/Desktop/LIC/MoneyPack -1.pdf", FileMode.Open, FileAccess.Read);
                    byte[] data = new byte[fs.Length];
                    fs.Read(data, 0, data.Length);
                    string base64 = Convert.ToBase64String(data);
                    //encodedfile = Base64.EncodeToUtf8InPlace(fs).toString();
                    //string base64 = "";
                    BinaryReader br = new BinaryReader(fs); //reads the binary files  
                    Byte[] bytes = br.ReadBytes((Int32)fs.Length); //counting the file length into bytes  
                    fs.Close();
                    // String sqlstring = "INSERT into Usertb (Id, Name, Gender, files) VALUES ('" + username.Id + "','" + username.Name + "','" + username.Gender + "','"+ base64 + "')";
                    String sqlstring = "INSERT into tbl_envelope (ApproverEmail, ApproverName, ApproverTitle, SenderEmail,SenderName,Status,UserId) VALUES ('" + ObjModel.ApproverEmail + "','" + ObjModel.ApproverName + "','" + ObjModel.ApproverTitle + "','" + ObjModel.SenderEmail + "','" + ObjModel.SenderName + "','" + ObjModel.Status + "','" + strResult + "')";
                    MySql.Data.MySqlClient.MySqlCommand cmd3 = new MySql.Data.MySqlClient.MySqlCommand(sqlstring, con);
                    cmd3.ExecuteNonQuery();
                    long id = cmd3.LastInsertedId;

                    string InsertFile = "Insert into tbl_envelope_document (EnvelopeId, DocumentName, DocumentContent) values ('" + id + "','" + ObjModel.FileName + "','" + base64 + "' )";
                    MySql.Data.MySqlClient.MySqlCommand cmd2 = new MySql.Data.MySqlClient.MySqlCommand(InsertFile, con);
                    cmd2.ExecuteNonQuery();

                    ESignDocumentGenerator objmodel = new ESignDocumentGenerator(GetDocuSignCredentials(), GetEmailTemplate(), GetDocuSignTemplate());

                    objmodel.GenerateDocument(ObjModel.ApproverName, ObjModel.ApproverEmail, Convert.ToInt32(id));

                    //  return RedirectToRoute("CreateDocument", "Home");


                    HttpResponseMessage response = new HttpResponseMessage();
                    response = Request.CreateResponse(HttpStatusCode.OK, "Authorized");
                    response.Headers.Add("Token", "bcyvtdvccc67749t23bedfuybeudf");
                    response.Headers.Add("TokenExpiry", "30");
                    response.Headers.Add("Access-Control-Expose-Headers", "Token,TokenExpiry");
                    return response;

                    con.Close();
                }
                else
                {
                    con.Close();

                    var message = new HttpResponseMessage(HttpStatusCode.NotAcceptable);
                    message.Content = new StringContent("Not Authorized User");
                    return message;
                }

            }
            else
            {
                con.Close();
                var message = new HttpResponseMessage(HttpStatusCode.NotAcceptable);
                message.Content = new StringContent("Not Authorized User");
                return message;
            }
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


                // HttpWebRequest req = WebRequest.Create(requestUri) as HttpWebRequest;

               
                //HttpRequestMessage re = new HttpRequestMessage();
                //var headers = re.Headers;

                //if (headers.Contains("TokenKey"))
                //{
                //    string token = headers.GetValues("TokenKey").First();

                //    if(token == TokenKey)
                //    {

                //    }
                //}

 

    }
}
