using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using SAMLTEST.Models;
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

        public IActionResult OnGet(string tenant, string policy, string issuer)
        {
            // Try to get values from the sessions, if none then use the default values
            this.Tenant = string.IsNullOrEmpty(HttpContext.Session.GetString("Tenant")) ? this.Tenant : HttpContext.Session.GetString("Tenant");
            this.Policy = string.IsNullOrEmpty(HttpContext.Session.GetString("Policy")) ? this.Policy : HttpContext.Session.GetString("Policy");
            this.Issuer = string.IsNullOrEmpty(HttpContext.Session.GetString("Issuer")) ? this.Issuer : HttpContext.Session.GetString("Issuer");
            // Override the values with the query string values
            if (!string.IsNullOrEmpty(tenant))
            {
                this.Tenant = tenant.Contains("onmicrosoft.com", StringComparison.OrdinalIgnoreCase) ? tenant : $"{tenant}.onmicrosoft.com"; ;
            }
            if (!string.IsNullOrEmpty(policy))
            {
                this.Policy = policy.StartsWith("B2C_1A_") ? policy : "B2C_1A_" + policy; ;
            }
            if (!string.IsNullOrEmpty(issuer))
            {
                this.Issuer = issuer;
            }
            // Save the values to the session for the future use
            if (null != this.Tenant) HttpContext.Session.SetString("Tenant", this.Tenant);
            if (null != this.Policy) HttpContext.Session.SetString("Policy", this.Policy);
            if (null != this.Issuer) HttpContext.Session.SetString("Issuer", this.Issuer);
            return Page();
        }

        /// <summary>
        /// This Post Action is used to Generate and POST the SAML Repsonse for and IDP initiated SSO
        /// </summary>
        public IActionResult OnPost(string tenant, string policy)
        {
            Policy = policy.StartsWith("B2C_1A_") ? policy : "B2C_1A_" + policy;
            Tenant = tenant.Contains("onmicrosoft.com", StringComparison.OrdinalIgnoreCase) ? tenant : $"{tenant}.onmicrosoft.com";
            var b2cLoginDomain = $"{Tenant.Split(".")[0]}.b2clogin.com";
            var relayState = $"{SAMLHelper.toB64(Tenant)}.{SAMLHelper.toB64(Policy)}.{SAMLHelper.toB64(Issuer)}";
            // To sign in or sign up a user through IdP-initiated flow, use the following URL:
            // https://<tenant-name>.b2clogin.com/<tenant-name>.onmicrosoft.com/<policy-name>/generic/login?EntityId=<app-identifier-uri>&RelayState=<relay-state>
            // source: https://learn.microsoft.com/en-us/azure/active-directory-b2c/saml-service-provider-options?pivots=b2c-custom-policy#configure-idp-initiated-flow
            var ACS = $"https://{b2cLoginDomain}/{Tenant}/{Policy}/generic/login?EntityId={Issuer}&RelayState={System.Web.HttpUtility.UrlEncode(relayState)}";

            SAMLResponse Resp = new SAMLResponse(ACS, "", SAMLHelper.GetThisURL(this), _configuration);
            var SAMLResponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(Resp.ToString()));

            return Content(SAMLHelper.GeneratePost(SAMLResponse, ACS, "SAMLResponse"), "text/html");
        }
    }
}