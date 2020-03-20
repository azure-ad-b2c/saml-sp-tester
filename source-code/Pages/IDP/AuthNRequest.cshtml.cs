using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using SAMLTEST.SAMLObjects;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace SAMLTEST.Pages.IDP
{
    /// <summary>
    /// This is the AuthN Request Page Model
    /// </summary>
    public class AuthNRequestModel : PageModel
    {
        public String RelayState { get; set; }
        public String ACS { get; set; }
        public String ID { get; private set; }
        [DisplayName("UserName")]
        public string UserName { get; set; }
        [DisplayName("Password")]
        public string Password { get; set; }
        public string SAMLResponse { get; set; }

        private readonly IConfiguration _configuration;

        /// <summary>
        /// This Constructor is used to retrieve the Appsettings data
        /// </summary>
        public AuthNRequestModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// This Get Action is used to Generate and POST the SAML Repsonse 
        /// based on a supplied AuthN Request
        /// </summary>
        public void OnGet(String SAMLRequest, String RelayState)
        { 
            this.RelayState = RelayState;

            String sml = SAMLHelper.Decompress(SAMLRequest);
            XmlDocument doc = new XmlDocument();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
            nsmgr.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");
            doc.LoadXml(sml);
            XmlElement root = doc.DocumentElement;

            ACS = root.SelectSingleNode("/samlp:AuthnRequest/@AssertionConsumerServiceURL", nsmgr).Value;
            ID = root.SelectSingleNode("/samlp:AuthnRequest/@ID", nsmgr).Value;

            string httpors = HttpContext.Request.IsHttps ? "https://" : "http://";
            string thisurl = httpors + HttpContext.Request.Host.Value;
            SAMLResponse Resp = new SAMLResponse(ACS, ID, thisurl, _configuration);
            this.SAMLResponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(Resp. ToString()));
            this.RelayState = RelayState;
        }


    }
}