using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
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
        public string Tenant { get; set; } = "azureadb2ctests";

        [DisplayName("Host Name"), Required]
        public string HostName { get; set; } = "azureadb2ctests.b2clogin.com";

        [DisplayName("B2C Policy"),Required]
        public string Policy { get; set; } = "B2C_1A_SignUpOrSignin_SamlApp_Local";
      

        [DisplayName("Issuer")]
        public string Issuer { get; set; } = "https://azureadb2ctests.onmicrosoft.com/samlAPPUITest";

        [DisplayName("DCInfo")]
        public string DCInfo { get; set; } = ""; 

        private readonly IConfiguration _configuration;

        /// <summary>
        /// This Constructor is used to retrieve the Appsettings data
        /// </summary>
        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult OnGet(string Tenant, string HostName, string Policy, string Issuer)
        {
            this.Tenant = HttpContext.Session.GetString("Tenant");
            this.HostName = HttpContext.Session.GetString("HostName");
            this.Policy = HttpContext.Session.GetString("Policy");
            this.Issuer = HttpContext.Session.GetString("Issuer");
            if (!string.IsNullOrEmpty(Tenant)) {
                this.Tenant = Tenant;
            }
            if (!string.IsNullOrEmpty(HostName))
            {
                this.HostName = HostName;
            }
            // if still null, build up hostname yourtenant.b2clogin.com from tenant name yourtenant.onmicrosoft.com 
            if (string.IsNullOrEmpty(this.HostName) )
            {
                string TenantName = this.Tenant.ToLower()?.Replace(".onmicrosoft.com", "");
                this.HostName = TenantName + ".b2clogin.com";
            }
            if (!string.IsNullOrEmpty(Policy)) {
                this.Policy = Policy;
            }
            if (!string.IsNullOrEmpty(Issuer)) {
                this.Issuer = Issuer;
            }
            if ( null != this.Tenant) HttpContext.Session.SetString("Tenant", this.Tenant);
            if ( null != this.HostName) HttpContext.Session.SetString("HostName", this.HostName);
            if ( null != this.Policy) HttpContext.Session.SetString("Policy", this.Policy);
            if ( null != this.Issuer ) HttpContext.Session.SetString("Issuer", this.Issuer);
            return Page();
        }

        /// <summary>
        /// This Post Action is used to Generate the AuthN Request and redirect to the B2C Login endpoint
        /// </summary>
        public IActionResult OnPost(string Tenant, string HostName, string Policy, string Issuer, string DCInfo, bool IsAzureAD)
        {
            if (string.IsNullOrEmpty(Policy) || IsAzureAD)
            {
                return SendAzureAdRequest(Tenant);
            }

            string SamlRequest = string.Empty;
            string b2cloginurl = HostName.ToLower();
            if (!String.IsNullOrEmpty(HostName))
            {
                b2cloginurl = HostName;
            }
            else if (!String.IsNullOrEmpty(this.Tenant) && this.Tenant.EndsWith(".onmicrosoft.com"))
            {
                string TenantName = Tenant.ToLower()?.Replace(".onmicrosoft.com", "");
                b2cloginurl = TenantName + ".b2clogin.com";
            }


            Policy = Policy.StartsWith("B2C_1A_") ? Policy : "B2C_1A_" + Policy;
            //Tenant = (Tenant.ToLower().Contains("onmicrosoft.com") || Tenant.ToLower().Contains(".net")) ? Tenant : Tenant + ".onmicrosoft.com";
            DCInfo = string.IsNullOrWhiteSpace(DCInfo) ? string.Empty : "&" + DCInfo;
            Issuer = string.IsNullOrWhiteSpace(Issuer) ? SAMLHelper.GetThisURL(this) : Issuer;

            if (null != Tenant) HttpContext.Session.SetString("Tenant", Tenant);
            if (null != b2cloginurl) HttpContext.Session.SetString("HostName", b2cloginurl);
            if (null != Policy) HttpContext.Session.SetString("Policy", Policy);
            if (null != Issuer) HttpContext.Session.SetString("Issuer", Issuer);

            string RelayState = SAMLHelper.toB64(Tenant) + "." + SAMLHelper.toB64(Policy) + "." + SAMLHelper.toB64(Issuer);

            if (!string.IsNullOrEmpty(DCInfo))
            {
                RelayState = RelayState + "." + SAMLHelper.toB64(DCInfo);
            }

            AuthnRequest AuthnReq;
            string URL = "https://" + b2cloginurl + "/" + Tenant + "/" + Policy + "/samlp/sso/login?" + DCInfo;
            AuthnReq = new AuthnRequest(URL, SAMLHelper.GetThisURL(this), Issuer);
            string cdoc = SAMLHelper.Compress(AuthnReq.ToString());
            URL = URL + "&SAMLRequest=" + System.Web.HttpUtility.UrlEncode(cdoc) + "&RelayState=" + System.Web.HttpUtility.UrlEncode(RelayState);
            return Redirect(URL);
        }

        public IActionResult SendAzureAdRequest(string Tenant)
        {
            AuthnRequest AuthnReq;
            AuthnReq = new AuthnRequest("https://login.microsoftonline.com/00000000-0000-0000-0000-000000000000/saml2", SAMLHelper.GetThisURL(this), string.Empty);

            string cdoc = SAMLHelper.Compress(AuthnReq.ToString());
            string URL = $"https://login.microsoftonline.com/00000000-0000-0000-0000-000000000000/saml2?SAMLRequest=" + System.Web.HttpUtility.UrlEncode(cdoc);
            return Redirect(URL);
        }

    }
}
