using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SAMLTEST.SAMLObjects
{
    [XmlRoot(ElementName = "LogoutRequest", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    public class LogoutRequest
    {
        // Empty Contructor required for Serialization
        public LogoutRequest() { }

        public LogoutRequest(string Destination, string thisurl, string sessionIndex, string NameId, string Issuer)
        {
            this.Destination = Destination;
            this.Issuer.Text = Issuer;
            this.NameIDPolicy.SPNameQualifier = thisurl;
            this.SessionIndex = sessionIndex;
        }

        [XmlAttribute(AttributeName = "ID")]
        //public string ID =  (new Random()).Next().ToString();
        public string ID = "_" + (new Random()).Next().ToString();
        [XmlAttribute(AttributeName = "Version")]
        public string Version = "2.0";
        [XmlAttribute(AttributeName = "IsPassive")]
        public string IsPassive = "false";

        [XmlAttribute(AttributeName = "IssueInstant")]
        //public string IssueInstant = DateTime.Now.AddMinutes(-5).ToString("o");
        public string IssueInstant = DateTime.Now.ToString("o");
        [XmlAttribute(AttributeName = "Destination")]
        public string Destination { get; set; } = "NoDestination";
        [XmlAttribute(AttributeName = "ProtocolBinding")]
        public string ProtocolBinding { get; set; } = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect";

        [XmlElement(ElementName = "Issuer", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion", Order = 1)]
        public Issuer Issuer { get; set; } = new Issuer();
        [XmlElement(ElementName = "NameIDPolicy", Order = 2)]
        public NameIDPolicy NameIDPolicy = new NameIDPolicy();

        [XmlElement(ElementName = "SessionIndex", Order = 3)]
        public string SessionIndex { get; set; }
        /// <summary>
        /// This method is used to ovewrite the To String Function
        /// This will serialise this object to XML
        /// </summary>
        public override string ToString()
        {
            // Serialize to XML
            String xml = SAMLHelper.Serialize(typeof(LogoutRequest), this);
            return xml;
        }
    }
}

