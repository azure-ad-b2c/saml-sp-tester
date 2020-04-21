﻿using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// This Post Action is used to Generate the AuthN Request and redirect to the B2C Login endpoint
        /// </summary>
        public IActionResult OnPost(string Tenant,string Policy, string Issuer, string DCInfo, bool IsAzureAD)
        {
            if (string.IsNullOrEmpty(Policy) || IsAzureAD)
            {
                return SendAzureAdRequest(Tenant);
            }

            String TenantId = Tenant.ToLower()?.Replace(".onmicrosoft.com", "");
            string SamlRequest = string.Empty;
            string b2cloginurl = TenantId + ".b2clogin.com";
            Policy = Policy.StartsWith("B2C_1A_") ? Policy : "B2C_1A_" + Policy;
            Tenant = (Tenant.ToLower().Contains("onmicrosoft.com") || Tenant.ToLower().Contains(".net")) ? Tenant : Tenant + ".onmicrosoft.com";
            DCInfo = string.IsNullOrWhiteSpace(DCInfo) ? string.Empty : "&" + DCInfo;
            Issuer = string.IsNullOrWhiteSpace(Issuer) ? SAMLHelper.GetThisURL(this) : Issuer;

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