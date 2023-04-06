using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SAMLTEST.SAMLObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SAMLTEST.Pages.SP
{
    /// <summary>
    /// This is the Assertion Consumer Page Model
    /// This page will be posted to from outside this application
    /// thus the Ignore Anti Forgery Token below
    /// </summary>
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class AssertionConsumerModel : PageModel
    {
        public string SessionId { get; private set; }

        public string SAMLResponse { get; private set; }
        public Dictionary<string, string> Attrsandvals { get; private set; }

        public string TenantId { get; private set; }
        public string PolicyId { get; private set; }
        public string NameId { get; private set; }

        public string DCInfo { get; private set; }
        public string Issuer { get; private set; }

        public IActionResult OnPost(string samlResponse, string relayState)
        {
            //Get Tenant, Policy, Issuer and DCInfo from RelayState
            if (!string.IsNullOrWhiteSpace(relayState))
            {
                string[] RelayStateBits = relayState.Split(".");
                this.TenantId = SAMLHelper.fromB64(RelayStateBits[0]);
                this.PolicyId = SAMLHelper.fromB64(RelayStateBits[1]);
                this.Issuer = SAMLHelper.fromB64(RelayStateBits[2]);
                if (RelayStateBits.Length > 3)
                {
                    this.DCInfo = SAMLHelper.fromB64(RelayStateBits[3]);
                    this.DCInfo = this.DCInfo.Replace("&", "");
                }
                else
                {
                    this.DCInfo = string.Empty;
                }
            }

            byte[] ENcSAMLByteArray = Convert.FromBase64String(samlResponse);
            var sml = System.Text.ASCIIEncoding.ASCII.GetString(ENcSAMLByteArray);
            var doc = new XmlDocument();
            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
            nsmgr.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");
            doc.LoadXml(sml);
            XmlElement root = doc.DocumentElement;

            string statusCode = root.SelectSingleNode("/samlp:Response/samlp:Status/samlp:StatusCode/@Value", nsmgr).Value;
            if (statusCode.Trim() != "urn:oasis:names:tc:SAML:2.0:status:Success")
            {
                string statusMessage = root.SelectSingleNode("/samlp:Response/samlp:Status/samlp:StatusMessage", nsmgr).InnerText;
                return Redirect("/Error?ErrorMessage=" + statusMessage);
            }

            XmlNodeList nodes = root.SelectNodes("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute", nsmgr);
            this.Attrsandvals = new Dictionary<string, string>();
            foreach (XmlNode node in nodes)
            {
                var attributeName = node.Attributes["Name"].Value;
                var val = string.Empty;
                if (node.HasChildNodes && node.ChildNodes.Count > 1)
                {
                    var values = node.ChildNodes.Cast<XmlNode>()
                                                .Select(item => item.InnerText)
                                                .ToList();
                    val = string.Join("<br>", values);
                }
                else
                {
                    val = node.InnerText;
                }
                this.Attrsandvals.Add(attributeName, val);
            }

            this.SAMLResponse = sml;
            this.SessionId = root.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AuthnStatement/@SessionIndex", nsmgr).Value;
            this.NameId = root.SelectSingleNode("/samlp:Response/saml:Assertion/saml:Subject/saml:NameID", nsmgr).InnerText;
            return Page();

        }
    }
}