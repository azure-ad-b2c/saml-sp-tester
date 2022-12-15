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
        public string RelayState { get; set; }
        public string ACS { get; set; }
        public string ID { get; private set; }
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
        /// This Get Action is used to Generate and POST the SAML Response
        /// based on a supplied AuthN Request
        /// </summary>
        public void OnGet(string SAMLRequest, string RelayState)
        {
            this.RelayState = RelayState;

            var sml = SAMLHelper.Decompress(SAMLRequest);
            var doc = new XmlDocument();
            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
            nsmgr.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");
            doc.LoadXml(sml);
            XmlElement root = doc.DocumentElement;

            ACS = root.SelectSingleNode("/samlp:AuthnRequest/@AssertionConsumerServiceURL", nsmgr).Value;
            ID = root.SelectSingleNode("/samlp:AuthnRequest/@ID", nsmgr).Value;

            var httpors = HttpContext.Request.IsHttps ? "https://" : "http://";
            var thisurl = httpors + HttpContext.Request.Host.Value;
            var response = new SAMLResponse(ACS, ID, thisurl, _configuration);
            this.SAMLResponse = Convert.ToBase64String(Encoding.UTF8.GetBytes(response.ToString()));
            this.RelayState = RelayState;
        }


    }
}