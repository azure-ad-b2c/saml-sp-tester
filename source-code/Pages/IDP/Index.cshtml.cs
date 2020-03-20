using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using SAMLTEST.SAMLObjects;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SAMLTEST.Pages.IDP
{
    /// <summary>
    /// This is the Index Page Model for the Identity Provider
    /// </summary>
    public class IndexModel : PageModel
    {
        [DisplayName("Tenant Name"), Required]
        public string Tenant { get; set; }
        [DisplayName("B2C Policy"), Required]
        public string Policy { get; set; } = "SAMLTEST";

        private readonly IConfiguration _configuration;

        /// <summary>
        /// This Constructor is used to retrieve the Appsettings data
        /// </summary>
        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// This Post Action is used to Generate and POST the SAML Repsonse for and IDP initiated SSO
        /// </summary>
        public IActionResult OnPost(string Tenant, string Policy)
        {

            string b2cloginurl = _configuration["SAMLTEST:b2cloginurl"];
            Policy = Policy.StartsWith("B2C_1A_") ? Policy : "B2C_1A_" + Policy;
            Tenant = Tenant.ToLower().Contains("onmicrosoft.com") ? Tenant : Tenant + ".onmicrosoft.com";


            string ACS = "https://" + b2cloginurl + "/te/" + Tenant + "/" + Policy + "/samlp/sso/assertionconsumer";

            SAMLResponse Resp = new SAMLResponse(ACS, "", SAMLHelper.GetThisURL(this), _configuration);
            string SAMLResponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(Resp.ToString()));

            return Content(SAMLHelper.GeneratePost(SAMLResponse,ACS,"SAMLResponse"),"text/html");
            

        }
    }
}