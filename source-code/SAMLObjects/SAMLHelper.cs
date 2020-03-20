using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using SAMLTEST.Pages.SP;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SAMLTEST.SAMLObjects
{
    /// <summary>
    /// This class Assists in some of the SAML Related Tasks
    /// like compressing / decompressin SAML MEssages
    /// </summary>
    public class SAMLHelper
    {
        /// <summary>
        /// This method deocompresses a SAML Message and returns it as a String.
        /// </summary>
        public static String Decompress(string EncSAML)
        {
            byte[] data = Convert.FromBase64String(EncSAML);
            var output = new MemoryStream();
            var compressedStream = new MemoryStream(data);

            using (DeflateStream zip = new DeflateStream(compressedStream, CompressionMode.Decompress, true))
            {
                byte[] buffer = new byte[1024];
                int nRead;
                while ((nRead = zip.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, nRead);
                }
                return System.Text.ASCIIEncoding.ASCII.GetString(output.ToArray());
            }

        }

        /// <summary>
        /// This method comresses an XML document as a String into a comrpessed SAML Message.
        /// </summary>
        public static string Compress(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            MemoryStream ms = new MemoryStream();

            using (DeflateStream zip = new DeflateStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
            }

            ms.Position = 0;
            MemoryStream outStream = new MemoryStream();

            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            byte[] gzBuffer = new byte[compressed.Length + 4];
            System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return Convert.ToBase64String(compressed);
        }

        /// <summary>
        /// Used to get the Web Url of theis application.
        /// </summary>
        internal static string GetThisURL(PageModel pmodel)
        {
            return $"{pmodel.HttpContext.Request.Scheme}://{pmodel.HttpContext.Request.Host}";
        }

        /// <summary>
        /// This method constructs a HTML Form for Posting.
        /// </summary>
        public static string GeneratePost(String SAMLRqXMLB64, String url, String Type= "SAMLRequest")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"<html><body onload='document.forms[""form""].submit()'>");
            sb.AppendFormat("<form name='form' action='{0}' method='post'>", url);
            sb.AppendFormat("<input type='hidden' name='{1}' value='{0}'>", SAMLRqXMLB64, Type);
            sb.Append("</form></body></html>");
            return sb.ToString();
        }


        /// <summary>
        /// A simple method to convert a string to Base64.
        /// </summary>
        public static string toB64(string text)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// A simple method to convert a string from Base64.
        /// </summary>
        public static string fromB64(string text)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(text);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>
        /// This method is used by the ToString overrides to serialise objects to an XML string.
        /// </summary>
        internal static string Serialize(Type t, object o)
        {
            XmlSerializer xsSubmit = new XmlSerializer(t);

            var xml = "";

            XmlSerializerNamespaces myNamespaces = new XmlSerializerNamespaces();
            myNamespaces.Add("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");
            myNamespaces.Add("saml", "urn:oasis:names:tc:SAML:2.0:assertion");

            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                Indent = false,
                OmitXmlDeclaration = false
            };

            using (var sww = new Utf8StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sww, settings))
                {
                    xsSubmit.Serialize(writer, o, myNamespaces);
                    xml = sww.ToString();
                }
            }
            return xml;
        }

    }

    /// <summary>
    /// This Class is used to override the Encoding to UTF8.
    /// </summary>
    public sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }

}
