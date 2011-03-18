﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;

namespace OnlineVideos.Sites
{
    public class EuroSportUtil : SiteUtilBase
    {

        [Category("OnlineVideosUserConfiguration"), Description("The tld for the eurosportplayer url, e.g. nl, co.uk or de")]
        string tld = null;

        [Category("OnlineVideosUserConfiguration"), Description("Email address of your eurosport account")]
        string emailAddress = null;
        [Category("OnlineVideosUserConfiguration"), Description("Password of your eurosport account")]
        string password = null;

        private string baseUrl;

        private XmlNamespaceManager nsmRequest;

        public override int DiscoverDynamicCategories()
        {
            if (tld == null || emailAddress == null || password == null)
                return 0;
            baseUrl = String.Format(@"http://www.eurosportplayer.{0}/", tld);

            CookieContainer cc = new CookieContainer();

            string url = baseUrl + "_wsplayer_/CRMService.asmx/LoginWithService";
            string postData = "u=" + emailAddress + "&p=" + password + "&r=0&s=ply";

            string cookies = @"ns_cookietest=true,ns_session=true";
            string[] myCookies = cookies.Split(',');
            foreach (string aCookie in myCookies)
            {
                string[] name_value = aCookie.Split('=');
                Cookie c = new Cookie();
                c.Name = name_value[0];
                c.Value = name_value[1];
                c.Expires = DateTime.Now.AddHours(1);
                c.Domain = new Uri(url).Host;
                cc.Add(c);
            }

            string post = GetWebDataFromPost(url, postData, cc);

            CookieContainer newcc = new CookieContainer();
            CookieCollection ccol = cc.GetCookies(new Uri(baseUrl));
            foreach (Cookie c in ccol)
                newcc.Add(c);

            string getData = GetWebData(baseUrl + "tv.shtml", newcc);

            Category category = new Category();
            category.Name = "Live TV";

            Match m = Regex.Match(getData, @"<param\sname=""InitParams""\svalue=""(?<value>[^&]*)&#xA;\s*lang=(?<lang>[^&]*)&#xA;\s*,geoloc=(?<geoloc>[^&]*)&#xA;\s*,realip=(?<realip>[^&]*)&#xA;\s*,ut=(?<ut>[^&]*)&#xA;\s*,ht=(?<ht>[^&]*)&#xA;\s*,vidid=(?<vidid>[^&]*)&#xA;\s*,cuvid=(?<cuvid>[^&]*)&#xA;\s*,prdid=(?<prdid>[^&]*)&");
            if (m.Success)
                category.Other = m;

            Settings.Categories.Add(category);
            Settings.DynamicCategoriesDiscovered = true;
            return Settings.Categories.Count;
        }

        public override List<VideoInfo> getVideoList(Category category)
        {
            Match m = (Match)category.Other;

            string post = String.Format(@"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"">
<s:Body xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
<FindDefaultProductShortsByCountryAndService xmlns=""http://tempuri.org/"">
<countryCode>{0}</countryCode>
<type>Live</type>
<partnerCode />
<languageId>{1}</languageId>
<sportId>{2}</sportId>
<realIp>{3}</realIp>
<service>1</service>
<userId>{4}</userId>
<hkey>{5}</hkey>
<responseLangId>{1}</responseLangId>
</FindDefaultProductShortsByCountryAndService>
</s:Body></s:Envelope>", tld.ToUpperInvariant(), m.Groups["lang"].Value, -1, m.Groups["realip"].Value,
                   m.Groups["ut"].Value, m.Groups["ht"].Value);

            string postData = GetWebDataFromPost("http://videoshop.ws.eurosport.com/PlayerProductService.asmx",
                post, @"SOAPAction: ""http://tempuri.org/FindDefaultProductShortsByCountryAndService""");

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(postData);

            nsmRequest = new XmlNamespaceManager(doc.NameTable);
            nsmRequest.AddNamespace("a", "http://tempuri.org/");

            XmlNodeList list = doc.SelectNodes("//a:PlayerProduct", nsmRequest);

            List<VideoInfo> videos = new List<VideoInfo>();
            foreach (XmlNode prod in list)
            {
                VideoInfo video = new VideoInfo();
                video.Title = null;
                XmlNodeList nodeList = prod.SelectNodes("a:livestreams/a:livestream", nsmRequest);
                foreach (XmlNode stream in nodeList)
                {
                    string name = stream.SelectSingleNode("a:name", nsmRequest).InnerText;
                    if (video.Title == null)
                        video.Title = name;
                    else
                    {
                        int ind = 0;
                        while (ind < video.Title.Length && ind < name.Length && video.Title[ind] == name[ind])
                            ind++;
                        video.Title = name.Substring(0, ind);
                    }
                }

                video.ImageUrl = String.Format(@"http://layout.eurosportplayer.{0}/i", tld) + prod.SelectSingleNode("a:vignetteurl", nsmRequest).InnerText;
                video.Description = prod.SelectSingleNode("a:channellivesublabel", nsmRequest).InnerText;
                video.Other = nodeList;

                videos.Add(video);
            }

            return videos;

        }

        public override string getUrl(VideoInfo video)
        {
            XmlNodeList streams = (XmlNodeList)video.Other;
            video.PlaybackOptions = new Dictionary<string, string>();
            foreach (XmlNode stream in streams)
            {
                string url = stream.SelectSingleNode("a:securedurl", nsmRequest).InnerText;
                XmlNode nameNode = stream.SelectSingleNode("a:label/a:name", nsmRequest);
                string name;
                if (nameNode != null)
                    name = stream.SelectSingleNode("a:label/a:name", nsmRequest).InnerText;
                else
                    name = String.Empty;
                video.PlaybackOptions.Add(name, url);
            }

            string resultUrl;
            if (video.PlaybackOptions.Count == 0) return "";// if no match, return empty url -> error
            else
            {
                // return first found url as default
                var enumer = video.PlaybackOptions.GetEnumerator();
                enumer.MoveNext();
                resultUrl = enumer.Current.Value;
            }
            if (video.PlaybackOptions.Count == 1) video.PlaybackOptions = null;// only one url found, PlaybackOptions not needed

            return resultUrl;
        }

        private static string GetWebDataFromPost(string url, string postData, string headerExtra)
        {
            Log.Debug("get webdata from {0}", url);

            // request the data
            byte[] data = Encoding.UTF8.GetBytes(postData);

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            if (request == null) return "";
            request.Method = "POST";
            request.ContentType = "text/xml; charset=utf-8";
            request.UserAgent = OnlineVideoSettings.Instance.UserAgent;
            request.Timeout = 15000;
            request.ContentLength = data.Length;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            request.Headers.Add(headerExtra);

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream responseStream;
                if (response.ContentEncoding.ToLower().Contains("gzip"))
                    responseStream = new System.IO.Compression.GZipStream(response.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                else if (response.ContentEncoding.ToLower().Contains("deflate"))
                    responseStream = new System.IO.Compression.DeflateStream(response.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                else
                    responseStream = response.GetResponseStream();

                Encoding encoding = Encoding.UTF8;
                encoding = Encoding.GetEncoding(response.CharacterSet.Trim(new char[] { ' ', '"' }));

                StreamReader reader = new StreamReader(responseStream, encoding, true);
                string str = reader.ReadToEnd();
                return str.Trim();
            }

        }


    }
}