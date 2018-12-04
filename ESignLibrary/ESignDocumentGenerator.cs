using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocuSign.eSign.Api;
using DocuSign.eSign.Model;
using ESignLibrary.Model;

namespace ESignLibrary
{
    public class ESignDocumentGenerator
    {
        private MySql.Data.MySqlClient.MySqlConnection con;

        public ESignDocumentGenerator(Credentials credentials, EmailTemplate emailTemplate, TemplateFacade docuSignTemplate)
        {
            this.ESignCredentials = credentials;
            this.EmailTemplate = emailTemplate;
            this.DocuSignTemplate = docuSignTemplate;
        }

        public Credentials ESignCredentials { get; set; }

        public EmailTemplate EmailTemplate { get; set; }

        public TemplateFacade DocuSignTemplate { get; set; }

        public void GenerateDocument(string name, string email, int LastInsertID)
        {
            var docuSignClient = new ESignClient(this.ESignCredentials);
            var accountId = docuSignClient.AccountId;

            // assign recipient to template role by setting name, email, and role name.  Note that the
            // template role name must match the placeholder role name saved in your account template.  
            var templateRoles = this.DocuSignTemplate.TemplateRoleNames.Select(m => new TemplateRole
            {
                Email = email,
                Name = name,
                RoleName = m
            }).ToList();

            // create a new envelope which we will use to send the signature request
            var envelope = new EnvelopeDefinition
            {
                EmailSubject = this.EmailTemplate.Subject,
                EmailBlurb = this.EmailTemplate.MessageBody,
                TemplateId = this.DocuSignTemplate.TemplateId,
                TemplateRoles = templateRoles,
                Status = "sent"
            };
            //Added by MK - START

            FileStream fs = new FileStream(@"C:/Users/ue/Desktop/LIC/MoneyPack -1.pdf", FileMode.Open, FileAccess.Read);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            //string base64 = Convert.ToBase64String(data);

            EnvelopeDefinition envDef = new EnvelopeDefinition();
            envDef.EmailSubject = this.EmailTemplate.Subject;

            // Add a document to the envelope
            Document doc = new Document();
            doc.DocumentBase64 = System.Convert.ToBase64String(data);
            doc.Name = "TestFile.pdf";
            doc.DocumentId = "1";

            envDef.Documents = new List<Document>();
            envDef.Documents.Add(doc);

            // Add a recipient to sign the document
            Signer signer = new Signer();
            signer.Email = email;
            signer.Name = name;
            signer.RecipientId = "1";

            // Create a |SignHere| tab somewhere on the document for the recipient to sign
            signer.Tabs = new Tabs();
            signer.Tabs.SignHereTabs = new List<SignHere>();
            SignHere signHere = new SignHere();
            signHere.DocumentId = "1";
            signHere.PageNumber = "1";
            signHere.RecipientId = "1";
            signHere.XPosition = "100";
            signHere.YPosition = "150";
            signer.Tabs.SignHereTabs.Add(signHere);

            envDef.Recipients = new Recipients();
            envDef.Recipients.Signers = new List<Signer>();
            envDef.Recipients.Signers.Add(signer);

            // set envelope status to "sent" to immediately send the signature request
            envDef.Status = "sent";

            //Added by MK - END



            // |EnvelopesApi| contains methods related to creating and sending Envelopes (aka signature requests)
            var envelopesApi = new EnvelopesApi();
            //var envelopeSummary = envelopesApi.CreateEnvelope(accountId, envelope);
            var envelopeSummary = envelopesApi.CreateEnvelope(accountId, envDef);

            if(LastInsertID != 0)
            {
                string mycon;
                mycon = "server=localhost;port=3306;database=esignapp;user=root;password=sql123";
                con = new MySql.Data.MySqlClient.MySqlConnection();
                con.ConnectionString = mycon;
                con.Open();
                string UpdateFil = "update tbl_envelope set EnvelopeId = '" + envelopeSummary.EnvelopeId + "', EnvelopeStatus = '" + envelopeSummary.Status + "' where id = '" + LastInsertID + "' ";
                MySql.Data.MySqlClient.MySqlCommand cmd2 = new MySql.Data.MySqlClient.MySqlCommand(UpdateFil, con);
                cmd2.ExecuteNonQuery();
            }



        }
    }
}
