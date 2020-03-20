using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SAMLTEST.SAMLObjects
{
    [XmlRoot(ElementName = "NameIDPolicy", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    public class NameIDPolicy
    {
        [XmlAttribute(AttributeName = "samlp", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string samlp = "urn:oasis:names:tc:SAML:2.0:protocol";
        [XmlAttribute(AttributeName = "SPNameQualifier")]
        public string SPNameQualifier { get; set; }
        [XmlAttribute(AttributeName = "AllowCreate")]
        public string AllowCreate = "true";
        [XmlAttribute(AttributeName = "Format")]
        // urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress
        //public string Format = "urn:oasis:names:tc:SAML:2.0:nameid-format:kerberos";
        public string Format = "urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress";
    }

    [XmlRoot(ElementName = "Issuer", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public class Issuer
    {
        [XmlAttribute(AttributeName = "saml", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string saml = "urn:oasis:names:tc:SAML:2.0:assertion";
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "AuthnRequest", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    public class AuthnRequest
    {
        // Empty Contructor required for Serialization
        public AuthnRequest()   {   }

        public AuthnRequest(string Destination, string thisurl, string Issuer)
        {
            this.Destination = Destination;
            //if (Startup.IsDevelopment)
            //{
            //   this.AssertionConsumerServiceURL = thisurl + "/SP/AssertionConsumer";
            //}
            //else
            //{
            //    if (thisurl.Contains("samltestapp2"))
            //    {
            //        this.AssertionConsumerServiceURL = "https://samltestapp2.azurewebsites.net/SP/AssertionConsumer";
            //    }
            //    else
            //    {
            //        this.AssertionConsumerServiceURL = "https://samltest20190427104731.azurewebsites.net/SP/AssertionConsumer";
            //    }
            //}

            this.AssertionConsumerServiceURL = thisurl + "/SP/AssertionConsumer";
            this.Issuer.Text = Issuer;

            this.NameIDPolicy.SPNameQualifier = thisurl;
        }

        [XmlAttribute(AttributeName = "ID")]
        
        public string ID = "_" + (new Random()).Next().ToString();
        [XmlAttribute(AttributeName = "Version")]
        public string Version = "2.0";
        [XmlAttribute(AttributeName = "IsPassive")]
        public string IsPassive = "false";
        [XmlAttribute(AttributeName = "ForceAuthn")]
        public string ForceAuthn = "false";
        [XmlAttribute(AttributeName = "IssueInstant")]
        //public string IssueInstant = DateTime.Now.AddMinutes(-5).ToString("o");
        public string IssueInstant = DateTime.Now.ToString("o");
        [XmlAttribute(AttributeName = "Destination")]
        public string Destination { get; set; } = "NoDestination";
        [XmlAttribute(AttributeName = "ProtocolBinding")]
        public string ProtocolBinding { get; set; } = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST";
        [XmlAttribute(AttributeName = "AssertionConsumerServiceURL")]
        public string AssertionConsumerServiceURL { get; set; }
        [XmlElement(ElementName = "Issuer", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", Order = 1)]
        public Issuer Issuer { get; set; } = new Issuer();
        [XmlElement(ElementName = "NameIDPolicy", Order = 2)]
        public NameIDPolicy NameIDPolicy = new NameIDPolicy();

        /// <summary>
        /// This method is used to ovewrite the To String Function
        /// This will serialise this object to XML
        /// </summary>
        public override string ToString()
        {
            // Serialize to XML
            String xml = SAMLHelper.Serialize(typeof(AuthnRequest), this);
            return xml;
        }
    }

}
