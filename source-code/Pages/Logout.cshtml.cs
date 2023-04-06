using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using SAMLTEST.SAMLObjects;
using System;
using System.ComponentModel;

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


        public IActionResult OnGet(string tenant, string policy, string sessionId, string nameId, string issuer, string dcInfo)
        {
            return (string.IsNullOrEmpty(tenant) || String.IsNullOrEmpty(policy))
                ? Page()
                : OnPost(tenant, policy, sessionId, nameId, issuer, dcInfo);
        }

        public IActionResult OnPost(string tenant, string policy, string sessionId, string nameId, string issuer, string dcInfo)
        {
            Policy = policy.StartsWith("B2C_1A_") ? policy : "B2C_1A_" + policy;
            Tenant = tenant.Contains("onmicrosoft.com", StringComparison.OrdinalIgnoreCase) ? tenant : $"{tenant}.onmicrosoft.com";
            Issuer = string.IsNullOrEmpty(issuer) ? this.Issuer : issuer;
            var b2cLoginDomain = $"{Tenant.Split(".")[0]}.b2clogin.com";
            var relayState = $"{SAMLHelper.toB64(Tenant)}.{SAMLHelper.toB64(Policy)}.{SAMLHelper.toB64(Issuer)}";

            if (!string.IsNullOrEmpty(dcInfo))
            {
                DCInfo = dcInfo.Replace("dc", "&dc").Replace("slice", "&slice");
            }

            var url = $"https://{b2cLoginDomain}/{Tenant}/{Policy}/samlp/sso/logout?{DCInfo}";
            var logoutRequest = new LogoutRequest(url, SAMLHelper.GetThisURL(this), sessionId, nameId, Issuer);
            var cdoc = SAMLHelper.Compress(logoutRequest.ToString());
            url = $"{url}&SAMLRequest={System.Web.HttpUtility.UrlEncode(cdoc)}";

            return Redirect(url);

        }
    }
}
