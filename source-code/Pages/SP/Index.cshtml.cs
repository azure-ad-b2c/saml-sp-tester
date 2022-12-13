using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using SAMLTEST.Models;
using SAMLTEST.SAMLObjects;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SAMLTEST.Pages.SP
{
    /// <summary>
    /// This is the Index Page Model for the Service Provider
    /// </summary>
    public class IndexModel : PageModel
    {
        //[DisplayName("Is Azure AD"), Required]
        //public bool IsAzureAD { get; set; }


        [DisplayName("Tenant Name"), Required]
        public string Tenant { get; set; }

        [DisplayName("Host Name"), Required]
        public string HostName { get; set; }

        [DisplayName("B2C Policy"), Required]
        public string Policy { get; set; }

        [DisplayName("Issuer")]
        public string Issuer { get; set; }

        [DisplayName("DCInfo")]
        public string DCInfo { get; set; }

        private readonly AzureAdB2C _azureAdB2C;

        /// <summary>
        /// This Constructor is used to retrieve the Appsettings data
        /// </summary>
        public IndexModel(IOptions<AzureAdB2C> options)
        {
            _azureAdB2C = options.Value;
            Tenant = _azureAdB2C.Tenant;
            HostName = _azureAdB2C.HostName;
            Policy = _azureAdB2C.Policy;
            Issuer = _azureAdB2C.Issuer;
            DCInfo = _azureAdB2C.DCInfo;
        }

        public IActionResult OnGet(string tenant, string hostName, string policy, string issuer)
        {
            // Try to get values from the sessions, if none then use the default values
            this.Tenant = string.IsNullOrEmpty(HttpContext.Session.GetString("Tenant")) ? this.Tenant : HttpContext.Session.GetString("Tenant");
            this.HostName = string.IsNullOrEmpty(HttpContext.Session.GetString("HostName")) ? this.HostName : HttpContext.Session.GetString("HostName");
            this.Policy = string.IsNullOrEmpty(HttpContext.Session.GetString("Policy")) ? this.Policy : HttpContext.Session.GetString("Policy");
            this.Issuer = string.IsNullOrEmpty(HttpContext.Session.GetString("Issuer")) ? this.Issuer : HttpContext.Session.GetString("Issuer");
            // Override the values with the query string values
            if (!string.IsNullOrEmpty(tenant))
            {
                this.Tenant = tenant;
            }
            if (!string.IsNullOrEmpty(hostName))
            {
                this.HostName = hostName;
            }
            // if still null, build up hostname yourtenant.b2clogin.com from tenant name yourtenant.onmicrosoft.com 
            if (string.IsNullOrEmpty(this.HostName))
            {
                string TenantName = this.Tenant.ToLower()?.Replace(".onmicrosoft.com", "");
                this.HostName = TenantName + ".b2clogin.com";
            }
            if (!string.IsNullOrEmpty(policy))
            {
                this.Policy = policy;
            }
            if (!string.IsNullOrEmpty(issuer))
            {
                this.Issuer = issuer;
            }
            // Save the values to the session for the future use
            if (null != this.Tenant) HttpContext.Session.SetString("Tenant", this.Tenant);
            if (null != this.HostName) HttpContext.Session.SetString("HostName", this.HostName);
            if (null != this.Policy) HttpContext.Session.SetString("Policy", this.Policy);
            if (null != this.Issuer) HttpContext.Session.SetString("Issuer", this.Issuer);
            return Page();
        }

        /// <summary>
        /// This Post Action is used to Generate the AuthN Request and redirect to the B2C Login endpoint
        /// </summary>
        public IActionResult OnPost(string tenant, string hostName, string policy, string issuer, string dcInfo, bool isAzureAD)
        {
            if (string.IsNullOrEmpty(policy) || isAzureAD)
            {
                return SendAzureAdRequest(tenant);
            }

            string b2cloginurl = hostName.ToLower();
            if (!string.IsNullOrEmpty(hostName))
            {
                b2cloginurl = hostName;
            }
            else if (!string.IsNullOrEmpty(this.Tenant) && this.Tenant.EndsWith(".onmicrosoft.com"))
            {
                string TenantName = tenant.ToLower()?.Replace(".onmicrosoft.com", "");
                b2cloginurl = TenantName + ".b2clogin.com";
            }


            policy = policy.StartsWith("B2C_1A_") ? policy : "B2C_1A_" + policy;
            tenant = (tenant.Contains("onmicrosoft.com", StringComparison.OrdinalIgnoreCase)
                        || tenant.Contains(".net", StringComparison.OrdinalIgnoreCase))
                         ? tenant
                         : $"{tenant}.onmicrosoft.com";
            dcInfo = string.IsNullOrWhiteSpace(dcInfo) ? string.Empty : $"&{dcInfo}";
            issuer = string.IsNullOrWhiteSpace(issuer) ? SAMLHelper.GetThisURL(this) : issuer;

            if (null != tenant) HttpContext.Session.SetString("Tenant", tenant);
            if (null != b2cloginurl) HttpContext.Session.SetString("HostName", b2cloginurl);
            if (null != policy) HttpContext.Session.SetString("Policy", policy);
            if (null != issuer) HttpContext.Session.SetString("Issuer", issuer);

            string RelayState = $"{SAMLHelper.toB64(tenant)}.{SAMLHelper.toB64(policy)}.{SAMLHelper.toB64(issuer)}";

            if (!string.IsNullOrEmpty(dcInfo))
            {
                RelayState = $"{RelayState}.{SAMLHelper.toB64(dcInfo)}";
            }

            AuthnRequest AuthnReq;
            string URL = $"https://{b2cloginurl}/{tenant}/{policy}/samlp/sso/login?{dcInfo}";
            AuthnReq = new AuthnRequest(URL, SAMLHelper.GetThisURL(this), issuer);
            string cdoc = SAMLHelper.Compress(AuthnReq.ToString());
            URL = $"{URL}&SAMLRequest={System.Web.HttpUtility.UrlEncode(cdoc)}&RelayState={System.Web.HttpUtility.UrlEncode(RelayState)}";
            return Redirect(URL);
        }

        public IActionResult SendAzureAdRequest(string tenant)
        {
            AuthnRequest AuthnReq;
            AuthnReq = new AuthnRequest("https://login.microsoftonline.com/00000000-0000-0000-0000-000000000000/saml2", SAMLHelper.GetThisURL(this), string.Empty);

            string cdoc = SAMLHelper.Compress(AuthnReq.ToString());
            string URL = $"https://login.microsoftonline.com/00000000-0000-0000-0000-000000000000/saml2?SAMLRequest=" + System.Web.HttpUtility.UrlEncode(cdoc);
            return Redirect(URL);
        }

    }
}
