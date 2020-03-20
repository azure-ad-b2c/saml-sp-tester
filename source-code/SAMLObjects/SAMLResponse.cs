using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace SAMLTEST.SAMLObjects
{
    [XmlRoot(ElementName = "Issuer", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public class SAMLResponseIssuer
    {
        [XmlAttribute(AttributeName = "Format")]
        public string Format { get; set; } = "urn:oasis:names:tc:SAML:2.0:nameid-format:entity";
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "StatusCode", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    public class StatusCode
    {
        [XmlAttribute(AttributeName = "Value")]
        public string Value { get; set; } = "urn:oasis:names:tc:SAML:2.0:status:Success";
    }

    [XmlRoot(ElementName = "Status", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    public class Status
    {
        [XmlElement(ElementName = "StatusCode", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
        public StatusCode StatusCode { get; set; } = new StatusCode();
    }

    [XmlRoot(ElementName = "NameID", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public class NameID
    {
        [XmlAttribute(AttributeName = "Format")]
        public string Format { get; set; } = "urn:oasis:names:tc:SAML:2.0:nameid-format:transient";
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "SubjectConfirmationData", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public class SubjectConfirmationData
    {
        [XmlAttribute(AttributeName = "NotOnOrAfter")]
        public string NotOnOrAfter { get; set; } = "2222-03-25T05:09:56.7045172Z";
        [XmlAttribute(AttributeName = "Recipient")]
        public string Recipient { get; set; }
        [XmlAttribute(AttributeName = "InResponseTo")]
        public string InResponseTo { get; set; }
    }

    [XmlRoot(ElementName = "SubjectConfirmation", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public class SubjectConfirmation
    {
        [XmlElement(ElementName = "SubjectConfirmationData", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public SubjectConfirmationData SubjectConfirmationData { get; set; } = new SubjectConfirmationData();
        [XmlAttribute(AttributeName = "Method")]
        public string Method { get; set; } = "urn:oasis:names:tc:SAML:2.0:cm:bearer";
    }

    [XmlRoot(ElementName = "Subject", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public class Subject
    {
        [XmlElement(ElementName = "NameID", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public NameID NameID { get; set; } = new NameID();
        [XmlElement(ElementName = "SubjectConfirmation", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public SubjectConfirmation SubjectConfirmation { get; set; } = new SubjectConfirmation();
    }

    [XmlRoot(ElementName = "AudienceRestriction", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public class AudienceRestriction
    {
        [XmlElement(ElementName = "Audience", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public string Audience { get; set; }
    }

    [XmlRoot(ElementName = "Conditions", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public class Conditions
    {
        [XmlElement(ElementName = "AudienceRestriction", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public AudienceRestriction AudienceRestriction { get; set; } = new AudienceRestriction();
        [XmlAttribute(AttributeName = "NotBefore")]
        public string NotBefore { get; set; }
        [XmlAttribute(AttributeName = "NotOnOrAfter")]
        public string NotOnOrAfter { get; set; }
    }

    [XmlRoot(ElementName = "AuthnContext", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public class AuthnContext
    {
        [XmlElement(ElementName = "AuthnContextClassRef", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public string AuthnContextClassRef { get; set; } = "urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport";
    }

    [XmlRoot(ElementName = "AuthnStatement", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public class AuthnStatement
    {
        [XmlElement(ElementName = "AuthnContext", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public AuthnContext AuthnContext { get; set; } = new AuthnContext();
        [XmlAttribute(AttributeName = "SessionIndex")]
        public string SessionIndex { get; set; }
        [XmlAttribute(AttributeName = "AuthnInstant")]
        public string AuthnInstant { get; set; } = DateTime.Now.ToString("o");
    }

    [XmlRoot(ElementName = "AttributeValue", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public class AttributeValue
    {
        // Empty Contructor required for Serialization
        public AttributeValue()  {  }
        public AttributeValue(string value)
        {
            this.Text = value;
        }

        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; } = "xs:string";
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Attribute", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public class Attribute
    {
        // Empty Contructor required for Serialization
        public Attribute() {  }

        public Attribute(string attrName, string Value)
        {
            this.AttributeValue = new AttributeValue(Value);
            this.Name = attrName;
            this.FriendlyName = Name;
        }

        [XmlElement(ElementName = "AttributeValue", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public AttributeValue AttributeValue { get; set; }
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "NameFormat")]
        public string NameFormat { get; set; } = "urn:oasis:names:tc:SAML:2.0:attrname-format:basic";
        [XmlAttribute(AttributeName = "FriendlyName")]
        public string FriendlyName { get; set; }
    }

    [XmlRoot(ElementName = "AttributeStatement", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public class AttributeStatement
    {
        [XmlElement(ElementName = "Attribute", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public List<Attribute> Attribute { get; set; } = new List<Attribute>();
        [XmlAttribute(AttributeName = "xs", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xs { get; set; }
    }

    [XmlRoot(ElementName = "Assertion", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
    public class Assertion
    {
        [XmlElement(ElementName = "Issuer", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public SAMLResponseIssuer Issuer { get; set; } = new SAMLResponseIssuer();
        [XmlElement(ElementName = "Subject", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public Subject Subject { get; set; } = new Subject();
        [XmlElement(ElementName = "Conditions", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public Conditions Conditions { get; set; } = new Conditions();
        [XmlElement(ElementName = "AuthnStatement", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public AuthnStatement AuthnStatement { get; set; } = new AuthnStatement();
        [XmlElement(ElementName = "AttributeStatement", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public AttributeStatement AttributeStatement { get; set; } = new AttributeStatement();
        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; } = "_" + (new Random()).Next();
        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; } = "2.0";
        [XmlAttribute(AttributeName = "IssueInstant")]
        public string IssueInstant { get; set; } = DateTime.Now.ToString("o");
    }

    [XmlRoot(ElementName = "Response", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
    public class SAMLResponse
    {
       
        // Empty Contructor required for Serialization
        public SAMLResponse()  {  }

        public SAMLResponse(string aCS, string iD, string thisurl, IConfiguration _configuration)
        {             
            String guid = Guid.NewGuid().ToString();
            this.Destination = aCS;
            this.Assertion.Subject.SubjectConfirmation.SubjectConfirmationData.Recipient = aCS;

            // If ID is empty then dont set the response to 
            // This enables IDP initiated SSO
            if (iD != "")
            {
                this.InResponseTo = iD;
                this.Assertion.Subject.SubjectConfirmation.SubjectConfirmationData.InResponseTo = iD;
            }

            this.Issuer.Text = thisurl;
            this.Assertion.Issuer.Text = thisurl;
            //Add custom attributes form Configuraiton
            this.Assertion.AttributeStatement.Attribute.Add(new Attribute("Title", _configuration["SAMLTest:SAMLUser:Title"]));
            this.Assertion.AttributeStatement.Attribute.Add(new Attribute("FirstName", _configuration["SAMLTest:SAMLUser:FirstName"]));
            this.Assertion.AttributeStatement.Attribute.Add(new Attribute("LastName", _configuration["SAMLTest:SAMLUser:LastName"]));
            this.Assertion.AttributeStatement.Attribute.Add(new Attribute("UID", _configuration["SAMLTest:SAMLUser:UID"]));
            this.Assertion.AttributeStatement.Attribute.Add(new Attribute("objectId", guid));

            this.Assertion.Conditions.AudienceRestriction.Audience = thisurl;
            this.Assertion.Subject.NameID.Text = guid;
        }

        [XmlElement(ElementName = "Issuer", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public SAMLResponseIssuer Issuer { get; set; } = new SAMLResponseIssuer();
        [XmlElement(ElementName = "Status", Namespace = "urn:oasis:names:tc:SAML:2.0:protocol")]
        public Status Status { get; set; } = new Status();
        [XmlElement(ElementName = "Assertion", Namespace = "urn:oasis:names:tc:SAML:2.0:assertion")]
        public Assertion Assertion { get; set; } = new Assertion();
        [XmlAttribute(AttributeName = "saml", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Saml { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; } = "_" + (new Random()).Next() ;
        [XmlAttribute(AttributeName = "InResponseTo")]
        public string InResponseTo { get; set; }
        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; } = "2.0";
        [XmlAttribute(AttributeName = "IssueInstant")]
        public string IssueInstant { get; set; } = DateTime.Now.ToString("o");
        [XmlAttribute(AttributeName = "Destination")]
        public string Destination { get; set; }
        [XmlAttribute(AttributeName = "samlp", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Samlp { get; set; }

        /// <summary>
        /// This method is used to ovewrite the To String Function
        /// This will serialise this object to XML
        /// </summary>
        public override string ToString()
        {
            // Serialize to XML
            String xml = SAMLHelper.Serialize(typeof(SAMLResponse), this);
            return xml;
        }
    }

}
