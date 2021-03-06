﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Xml;

namespace ESign.Controllers
{
    public class TestController : ApiController
    {
        public void Post(HttpRequestMessage request)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(request.Content.ReadAsStreamAsync().Result);
            
            var mgr = new XmlNamespaceManager(xmldoc.NameTable);
            mgr.AddNamespace("a", "http://www.docusign.net/API/3.0");
           // mgr.AddNamespace("a", "https://www.docusign.com/supportdocs/api-docs-assets/DocuSign-REST-Recipes/010-webhook/example_completed_notification.xml");

            XmlNode envelopeStatus = xmldoc.SelectSingleNode("//a:EnvelopeStatus", mgr);
            XmlNode envelopeId = envelopeStatus.SelectSingleNode("//a:EnvelopeID", mgr);
            XmlNode status = envelopeStatus.SelectSingleNode("./a:Status", mgr);
            if (envelopeId != null)
            {
                System.IO.File.WriteAllText(HttpContext.Current.Server.MapPath("~/Documents/" +
                    envelopeId.InnerText + "_" + status.InnerText + "_" + Guid.NewGuid() + ".xml"), xmldoc.OuterXml);
            }

            if (status.InnerText == "Completed")
            {
                // Loop through the DocumentPDFs element, storing each document.

                XmlNode docs = xmldoc.SelectSingleNode("//a:DocumentPDFs", mgr);
                foreach (XmlNode doc in docs.ChildNodes)
                {
                    string documentName = doc.ChildNodes[0].InnerText; // pdf.SelectSingleNode("//a:Name", mgr).InnerText;
                    string documentId = doc.ChildNodes[2].InnerText; // pdf.SelectSingleNode("//a:DocumentID", mgr).InnerText;
                    string byteStr = doc.ChildNodes[1].InnerText; // pdf.SelectSingleNode("//a:PDFBytes", mgr).InnerText;

                    System.IO.File.WriteAllText(HttpContext.Current.Server.MapPath("~/Documents/" + envelopeId.InnerText + "_" + documentId + "_" + documentName), byteStr);
                }
            }
        }
    }
}
