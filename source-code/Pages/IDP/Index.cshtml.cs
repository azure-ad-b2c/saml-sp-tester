using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using SAMLTEST.Models;
using SAMLTEST.SAMLObjects;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml;

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
        public string Policy { get; set; }
        [DisplayName("Issuer"), Required]
        public string Issuer { get; set; }
        private readonly IConfiguration _configuration;

        /// <summary>
        /// This Constructor is used to retrieve the Appsettings data
        /// </summary>
        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
            var azureAdB2C = new AzureAdB2C();
            configuration.GetSection(AzureAdB2C.ConfigurationName).Bind(azureAdB2C);
            Tenant = azureAdB2C.Tenant;
            Policy = azureAdB2C.Policy;
            Issuer = azureAdB2C.Issuer;
        }

        /// <summary>
        /// This Post Action is used to Generate and POST the SAML Repsonse for and IDP initiated SSO
        /// </summary>
        public IActionResult OnPost(string Tenant, string Policy)
        {
            Policy = Policy.StartsWith("B2C_1A_") ? Policy : "B2C_1A_" + Policy;
            Tenant = Tenant.Contains("onmicrosoft.com", StringComparison.OrdinalIgnoreCase) ? $"{Tenant}.onmicrosoft.com" : Tenant;
            var relayState = $"{SAMLHelper.toB64(Tenant)}.{SAMLHelper.toB64(Policy)}.{SAMLHelper.toB64(Issuer)}";
            // To sign in or sign up a user through IdP-initiated flow, use the following URL:
            // https://<tenant-name>.b2clogin.com/<tenant-name>.onmicrosoft.com/<policy-name>/generic/login?EntityId=<app-identifier-uri>&RelayState=<relay-state>
            // source: https://learn.microsoft.com/en-us/azure/active-directory-b2c/saml-service-provider-options?pivots=b2c-custom-policy#configure-idp-initiated-flow
            var ACS = $"https://{Tenant}.b2clogin.com/{Tenant}.onmicrosoft.com/{Policy}/generic/login?EntityId={Issuer}&RelayState={System.Web.HttpUtility.UrlEncode(relayState)}";
            //var ACS = $"https://{b2cloginurl}/te/{Tenant}/{Policy}/samlp/sso/assertionconsumer";

            SAMLResponse Resp = new SAMLResponse(ACS, "", SAMLHelper.GetThisURL(this), _configuration);
            var SAMLResponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(Resp.ToString()));

            return Content(SAMLHelper.GeneratePost(SAMLResponse, ACS, "SAMLResponse"), "text/html");
        }
    }
}