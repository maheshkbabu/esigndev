using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESignLibrary.Model
{
    public class TemplateFacade
    {
        public TemplateFacade(string templateId, IList<string> templateRoleNames)
        {
            this.TemplateId = templateId;
            this.TemplateRoleNames = templateRoleNames;
        }

        public IList<string> TemplateRoleNames { get; set; }

        public string TemplateId { get; set; }
    }
}
