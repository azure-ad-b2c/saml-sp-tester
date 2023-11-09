using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SAMLTEST.SAMLObjects
{
    /// <summary>
    /// This class assists in some of the SAML related tasks like compressing / decompressing SAML messages.
    /// </summary>
    public class SAMLHelper
    {
        /// <summary>
        /// This method decompresses a SAML Message and returns it as a string.
        /// </summary>
        public static string Decompress(string encodedSaml)
        {
            var data = Convert.FromBase64String(encodedSaml);
            var output = new MemoryStream();
            var compressedStream = new MemoryStream(data);

            using (var zip = new DeflateStream(compressedStream, CompressionMode.Decompress, true))
            {
                var buffer = new byte[1024];
                int nRead;
                while ((nRead = zip.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, nRead);
                }
                return Encoding.ASCII.GetString(output.ToArray());
            }

        }

        /// <summary>
        /// This method compresses an XML document as a string into a compressed SAML message.
        /// </summary>
        public static string Compress(string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            var ms = new MemoryStream();

            using (var zip = new DeflateStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
            }

            ms.Position = 0;
            
            var compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            var gzBuffer = new byte[compressed.Length + 4];
            Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return Convert.ToBase64String(compressed);
        }

        /// <summary>
        /// Used to get the Web Url of theis application.
        /// </summary>
        internal static string GetThisURL(PageModel pageModel)
        {
            return $"{pageModel.HttpContext.Request.Scheme}://{pageModel.HttpContext.Request.Host}";
        }

        /// <summary>
        /// This method constructs a HTML Form for Posting.
        /// </summary>
        public static string GeneratePost(string samlRequestXmlBase64, string url, string type= "SAMLRequest")
        {
            var sb = new StringBuilder();
            sb.AppendFormat(@"<html><body onload='document.forms[""form""].submit()'>");
            sb.AppendFormat("<form name='form' action='{0}' method='post'>", url);
            sb.AppendFormat("<input type='hidden' name='{1}' value='{0}'>", samlRequestXmlBase64, type);
            sb.Append("</form></body></html>");
            return sb.ToString();
        }


        /// <summary>
        /// A simple method to convert a string to Base64.
        /// </summary>
        public static string ToBase64(string text)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// A simple method to convert a string from Base64.
        /// </summary>
        public static string FromBase64(string text)
        {
            var base64EncodedBytes = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>
        /// This method is used by the ToString overrides to serialise objects to an XML string.
        /// </summary>
        internal static string Serialize(Type t, object o)
        {
            var xsSubmit = new XmlSerializer(t);

            var myNamespaces = new XmlSerializerNamespaces();
            myNamespaces.Add("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");
            myNamespaces.Add("saml", "urn:oasis:names:tc:SAML:2.0:assertion");

            var settings = new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                Indent = false,
                OmitXmlDeclaration = false
            };

            string xml;
            using (var sww = new Utf8StringWriter())
            {
                using (var writer = XmlWriter.Create(sww, settings))
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
