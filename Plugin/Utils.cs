﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;
using System.Xml;

namespace OnlineVideos
{
    public static class Utils
    {
        public static string ToFriendlyCase(string PascalString)
        {
            return Regex.Replace(PascalString, "(?!^)([A-Z])", " $1");
        }

        public static string ReplaceEscapedUnicodeCharacter(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                input,
                @"(?:\\|%)[uU]([0-9A-Fa-f]{4})",
                delegate(System.Text.RegularExpressions.Match match)
                {
                    return ((char)Int32.Parse(match.Value.Substring(2), System.Globalization.NumberStyles.HexNumber)).ToString();
                });
        }

        public static DateTime UNIXTimeToDateTime(double unixTime)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTime).ToLocalTime();
        }

        public static void SiteSettingsToXml(SerializableSettings sites, Stream stream)
        {
            MemoryStream xmlMem = new MemoryStream();
            System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(typeof(SerializableSettings));
            dcs.WriteObject(xmlMem, sites);
            xmlMem.Position = 0;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlMem);

            Stream xslt = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("OnlineVideos.Configuration.ExportSiteSettings.xslt");
            System.Xml.Xsl.XslCompiledTransform xsltTransform = new System.Xml.Xsl.XslCompiledTransform();
            xsltTransform.Load(XmlReader.Create(xslt));
            
            xsltTransform.Transform(xmlDoc, null, stream);
            stream.Flush();
        }

        public static IList<SiteSettings> SiteSettingsFromXml(string siteXml)
        {
            siteXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<OnlineVideoSites xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
<Sites>
" + siteXml + @"
</Sites>
</OnlineVideoSites>";
            return SiteSettingsFromXml(new System.IO.StringReader(siteXml));
        }

        public static IList<SiteSettings> SiteSettingsFromXml(TextReader reader)
        {
            XmlDocument sitesXmlDoc = new XmlDocument();
            sitesXmlDoc.Load(reader);

            Stream xslt = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("OnlineVideos.Configuration.ImportSiteSettings.xslt");
            System.Xml.Xsl.XslCompiledTransform xsltTransform = new System.Xml.Xsl.XslCompiledTransform();
            xsltTransform.Load(XmlReader.Create(xslt));
            MemoryStream ms = new MemoryStream();
            xsltTransform.Transform(sitesXmlDoc, null, ms);
            ms.Flush();
            ms.Position = 0;

            System.Runtime.Serialization.DataContractSerializer dcs2 = new System.Runtime.Serialization.DataContractSerializer(typeof(SerializableSettings));
            XmlReader xr = XmlReader.Create(ms);
            xr.MoveToContent();
            SerializableSettings s = dcs2.ReadObject(xr) as SerializableSettings;
            return s != null ? s.Sites : null;
        }

        public static string PlainTextFromHtml(string input)
        {
            string result = input;
            if (!string.IsNullOrEmpty(result))
            {
                // decode HTML escape character
                result = System.Web.HttpUtility.HtmlDecode(result);

                // Replace &nbsp; with space
                result = Regex.Replace(result, @"&nbsp;", " ", RegexOptions.Multiline);

                // Remove double spaces
                result = Regex.Replace(result, @"  +", "", RegexOptions.Multiline);

                // Replace <br/> with \n
                result = Regex.Replace(result, @"< *br */*>", "\n", RegexOptions.IgnoreCase & RegexOptions.Multiline);

                // Remove remaining HTML tags                
                result = Regex.Replace(result, @"<[^>]*>", "", RegexOptions.Multiline);

                // Remove whitespace at the beginning and end
                result = result.Trim();
            }
            return result;
        }

        public static string[] Tokenize(string text, bool dropToken, params string[] tokens)
        {
            if( tokens.Length > 0){

                string regex = @"([";
                foreach (string s in tokens)
                    regex += s;
                regex += "])";
                Regex RE = new Regex(regex);
                if (dropToken)
                {
                    string output = RE.Replace(text, " ");
                    return (new Regex(@"\s").Split(output)); 
                }
                else
                    return (RE.Split(text));
            }
            return null;
        }

        /// <summary>
        /// Method to change the AllowUnsafeHeaderParsing property of HttpWebRequest.
        /// </summary>
        /// <param name="setState"></param>
        /// <returns></returns>
        public static bool SetAllowUnsafeHeaderParsing(bool setState)
        {
            try
            {
                //Get the assembly that contains the internal class
                Assembly aNetAssembly = Assembly.GetAssembly(typeof(System.Net.Configuration.SettingsSection));
                if (aNetAssembly == null)
                    return false;

                //Use the assembly in order to get the internal type for the internal class
                Type aSettingsType = aNetAssembly.GetType("System.Net.Configuration.SettingsSectionInternal");
                if (aSettingsType == null)
                    return false;

                //Use the internal static property to get an instance of the internal settings class.
                //If the static instance isn't created allready the property will create it for us.
                object anInstance = aSettingsType.InvokeMember("Section",
                                                                BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.NonPublic,
                                                                null, null, new object[] { });
                if (anInstance == null)
                    return false;

                //Locate the private bool field that tells the framework is unsafe header parsing should be allowed or not
                FieldInfo aUseUnsafeHeaderParsing = aSettingsType.GetField("useUnsafeHeaderParsing", BindingFlags.NonPublic | BindingFlags.Instance);
                if (aUseUnsafeHeaderParsing == null)
                    return false;

                // and finally set our setting
                aUseUnsafeHeaderParsing.SetValue(anInstance, setState);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Unsafe header parsing setting change failed: " + ex.ToString());
                return false;
            }
        }

        public static string DictionaryToString(Dictionary<string, string> dic)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings() { Encoding = Encoding.UTF8, Indent = true, OmitXmlDeclaration = true };
            using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(sb, settings))
            {
                writer.WriteStartElement("dictionary");
                foreach (string key in dic.Keys)
                {
                    writer.WriteStartElement("item");
                    writer.WriteStartElement("key");
                    writer.WriteCData(key);
                    writer.WriteEndElement();
                    writer.WriteStartElement("value");
                    writer.WriteCData(dic[key]);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.Flush();
                writer.Close();
            }
            return sb.ToString();
        }

        public static Dictionary<string, string> DictionaryFromString(string input)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(new System.IO.StringReader(input)))
            {
                bool wasEmpty = reader.IsEmptyElement;
                reader.Read();
                if (wasEmpty) return null;
                reader.ReadStartElement("dictionary");
                while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
                {
                    reader.ReadStartElement("item");
                    reader.ReadStartElement("key");
                    string key = reader.ReadContentAsString();
                    reader.ReadEndElement();
                    reader.ReadStartElement("value");
                    string value = reader.ReadContentAsString();
                    reader.ReadEndElement();
                    dic.Add(key, value);
                    reader.ReadEndElement();
                    reader.MoveToContent();
                }
                reader.ReadEndElement();
            }
            return dic;
        }

    }
}
