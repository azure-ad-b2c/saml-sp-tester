using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using SAMLTEST.SAMLObjects;

namespace SAMLTEST.Pages
{
    public class B2CPolicyModel : PageModel
    {
        [DisplayName("Tenant Name"), Required]
        public string Tenant { get; set; }
        [DisplayName("B2C Policy"), Required]
        public string Policy { get; set; } = "SAMLTEST";

        public IActionResult OnGet(String Tenant, String Policy)
        {
            if (String.IsNullOrEmpty(Tenant) || String.IsNullOrEmpty(Policy))
                return Page();
            else
                return OnPost(Tenant, Policy);
        }

        public IActionResult OnPost(String Tenant, String Policy)
        {
            Policy = Policy.StartsWith("B2C_1A_") ? Policy : "B2C_1A_" + Policy;
            Tenant = Tenant.ToLower().Contains("onmicrosoft.com") ? Tenant : Tenant + ".onmicrosoft.com";

            Assembly _assembly = Assembly.GetExecutingAssembly();
            Stream polstream = _assembly.GetManifestResourceStream("SAMLTEST.B2CPolicyTemplate.xml");
            StreamReader _textStreamReader = new StreamReader(polstream);
            String Polfile = _textStreamReader.ReadToEnd();
            Polfile = Polfile.Replace("%TENANTID%",Tenant);
            Polfile = Polfile.Replace("%POLICYID%", Policy);
            Polfile = Polfile.Replace("%THISWEBAPP%", SAMLHelper.GetThisURL(this));
            //Comment out the below line if you would prefer to show on the page
            Response.Headers.Add("Content-Disposition", "attachment; filename" + Policy + ".xml");
            return Content(Polfile, "text/xml");
        }
    }
}