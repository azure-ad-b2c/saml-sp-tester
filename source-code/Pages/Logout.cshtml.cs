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
using Microsoft.Extensions.Configuration;

namespace SAMLTEST.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly IConfiguration _configuration;

        [DisplayName("Tenant Name")]
        public string Tenant { get; set; } 
        [DisplayName("B2C Policy")]
        public string Policy { get; set; }

        [DisplayName("Issuer")]
        public string Issuer { get; set; }

        [DisplayName("DCInfo")]
        public string DCInfo { get; set; }

        /// <summary>
        /// This Constructor is used to retrieve the Appsettings data
        /// </summary>
        public LogoutModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public IActionResult OnGet(string Tenant, string Policy, string SessionId,string NameId, string Issuer, string DCInfo)
        {
            if (String.IsNullOrEmpty(Tenant) || String.IsNullOrEmpty(Policy))
                return Page();
            else
                return OnPost(Tenant, Policy, SessionId, NameId, Issuer, DCInfo);
        }

        public IActionResult OnPost(string Tenant, string Policy, string SessionId, string NameId, string Issuer, string DCInfo)
        {
            string b2cloginurl = Tenant.Split('.')[0] + ".b2clogin.com";

            if (!string.IsNullOrEmpty(DCInfo))
            {
                DCInfo = DCInfo.Replace("dc", "&dc");
                DCInfo = DCInfo.Replace("slice", "&slice");
            }

            Policy = Policy.StartsWith("B2C_1A_") ? Policy : "B2C_1A_" + Policy;
            Tenant = (Tenant.ToLower().Contains("onmicrosoft.com") || Tenant.ToLower().Contains("ccsctp.net")) ? Tenant : Tenant + ".onmicrosoft.com";
            string URL = "https://" + b2cloginurl + "/te/" + Tenant + "/" + Policy + "/samlp/sso/logout?" + DCInfo;

            LogoutRequest logoutRequest = new LogoutRequest(URL, SAMLHelper.GetThisURL(this), SessionId, NameId, Issuer);
            string cdoc = SAMLHelper.Compress(logoutRequest.ToString());
            URL = URL  + "&SAMLRequest=" + System.Web.HttpUtility.UrlEncode(cdoc);
            
            return Redirect(URL);

        }
    }
}
