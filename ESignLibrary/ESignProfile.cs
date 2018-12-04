using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESignLibrary.Model;
using DocuSign.eSign.Api;
using DocuSign.eSign.Model;

namespace ESignLibrary
{
    public class ESignProfile
    {
        public ESignProfile(Credentials credentials)
        {
            this.ESignCredentials = credentials;
        }

        public Credentials ESignCredentials { get; set; }

        public IEnumerable<Envelope> ListEnvelopes(int numberOfItems)
        {
            var docuSignClient = new ESignClient(this.ESignCredentials);
            var accountId = docuSignClient.AccountId;

            var fromDate = DateTime.UtcNow;
            fromDate = fromDate.AddDays(-30);
            string fromDateStr = fromDate.ToString("o");

            // set a filter for the envelopes we want returned using the fromDate and count properties
            var options = new EnvelopesApi.ListStatusChangesOptions()
            {
                count = numberOfItems.ToString(),
                fromDate = fromDateStr
            };

            EnvelopesApi envelopesApi = new EnvelopesApi();
            return envelopesApi.ListStatusChanges(accountId, options).Envelopes;
        }
    }
}
