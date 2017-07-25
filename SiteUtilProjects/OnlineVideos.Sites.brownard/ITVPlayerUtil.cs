﻿using HtmlAgilityPack;
using OnlineVideos.Sites.Brownard.Extensions;
using OnlineVideos.Sites.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace OnlineVideos.Sites
{
    public class ITVPlayerUtil : SiteUtilBase
    {
        #region Site Config
        [Category("OnlineVideosUserConfiguration"), Description("Select stream automatically?")]
        protected bool AutoSelectStream = false;
        [Category("OnlineVideosUserConfiguration"), Description("Proxy to use for WebRequests (must be in the UK). Define like this: 83.84.85.86:8116")]
        string proxy = null;
        [Category("OnlineVideosUserConfiguration"), Description("If your proxy requires a username, set it here.")]
        string proxyUsername = null;
        [Category("OnlineVideosUserConfiguration"), Description("If your proxy requires a password, set it here.")]
        string proxyPassword = null;
        [Category("OnlineVideosConfiguration"), Description("Regular Expression used on a video thumbnail for matching a string to be replaced for higher quality")]
        protected string thumbReplaceRegExPattern;
        [Category("OnlineVideosConfiguration"), Description("The string used to replace the match if the pattern from the thumbReplaceRegExPattern matched")]
        protected string thumbReplaceString;
        [Category("OnlineVideosUserConfiguration"), Description("Whether to download subtitles")]
        protected bool RetrieveSubtitles = false;
        [Category("OnlineVideosUserConfiguration"), Description("Whether to retrieve current program info for live streams.")]
        protected bool retrieveTVGuide = true;
        [Category("OnlineVideosConfiguration"), Description("The layout to use to display TV Guide info, possible wildcards are <nowtitle>,<nowdescription>,<nowstart>,<nowend>,<nexttitle>,<nextstart>,<nextend>,<newline>")]
        protected string tvGuideFormatString;// = "Now: <nowtitle> - <nowstart> - <nowend><newline>Next: <nexttitle> - <nextstart> - <nextend><newline><nowdescription>";        
        #endregion

        #region Consts
        const string SOAP_TEMPLATE = @"<?xml version='1.0' encoding='utf-8'?>
<SOAP-ENV:Envelope xmlns:SOAP-ENV='http://schemas.xmlsoap.org/soap/envelope/' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
    <SOAP-ENV:Body>
	    <tem:GetPlaylist xmlns:tem='http://tempuri.org/' xmlns:itv='http://schemas.datacontract.org/2004/07/Itv.BB.Mercury.Common.Types' xmlns:com='http://schemas.itv.com/2009/05/Common'>
	        <tem:request>
                {0}
		        <itv:RequestGuid>FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF</itv:RequestGuid>
		        <itv:Vodcrid>
		            <com:Id>{1}</com:Id>
		            <com:Partition>itv.com</com:Partition>
		        </itv:Vodcrid>
	        </tem:request>
	        <tem:userInfo>
		        <itv:GeoLocationToken>
		            <itv:Token/>
		        </itv:GeoLocationToken>
		        <itv:RevenueScienceValue>scc=true; svisit=1; sc4=Other</itv:RevenueScienceValue>
	        </tem:userInfo>
	        <tem:siteInfo>
		        <itv:Area>ITVPLAYER.VIDEO</itv:Area>
		        <itv:Platform>DotCom</itv:Platform>
		        <itv:Site>ItvCom</itv:Site>
	        </tem:siteInfo>
            <tem:deviceInfo> 
                <itv:ScreenSize>Big</itv:ScreenSize> 
            </tem:deviceInfo>
	    </tem:GetPlaylist>
	</SOAP-ENV:Body>
</SOAP-ENV:Envelope>";
        #endregion

        #region Regex
        static readonly Regex imageRegex = new Regex(@"<source srcset=""([^""]*)""");
        static readonly Regex numberRegex = new Regex(@"\d+");
        static readonly Regex singleVideoImageRegex = new Regex(@"background-image: url\('(.*?)'\)");

        //Search
        static readonly Regex searchRegex = new Regex(@"<div class=""search-wrapper"">.*?<div class=""search-result-image"">[\s\n]*(<a.*?><img.*?src=""(.*?)"")?.*?<h4 class=""programme-title""><a href=""(.*?)"">(.*?)</a>.*?<div class=""programme-description"">[\s\n]*(.*?)</div>", RegexOptions.Singleline);
        //ProductionId
        static readonly Regex productionIdRegex = new Regex(@"data-video-production-id=""(.*?)""");  //new Regex(@"data-video-id=""(.*?)""");
        #endregion

        #region SiteUtil Overrides
        DateTime lastRefresh = DateTime.MinValue;
        public override int DiscoverDynamicCategories()
        {
            if ((DateTime.Now - lastRefresh).TotalMinutes > 15)
            {
                foreach (Category cat in Settings.Categories)
                {
                    if (cat is RssLink)
                    {
                        cat.HasSubCategories = true;
                        cat.SubCategoriesDiscovered = false;
                    }
                    if (string.IsNullOrEmpty(cat.Thumb))
                        cat.Thumb = string.Format(@"{0}\Icons\{1}.png", OnlineVideoSettings.Instance.ThumbsDir, Settings.Name);
                }
                lastRefresh = DateTime.Now;
            }
            return Settings.Categories.Count;
        }

        public override int DiscoverSubCategories(Category parentCategory)
        {
            return getShowsList(parentCategory);
        }

        public override List<VideoInfo> GetVideos(Category category)
        {
            Group group = category as Group;
            if (group != null)
                return getLiveStreams(group);
            return getShowsVideos(category);
        }

        public override List<string> GetMultipleVideoUrls(VideoInfo video, bool inPlaylist = false)
        {
            string url = GetVideoUrl(video);
            if (inPlaylist)
                video.PlaybackOptions.Clear();
            return new List<string>() { url };
        }

        public override string GetVideoUrl(VideoInfo video)
        {
            bool isLiveStream = video.VideoUrl.StartsWith("sim");
            string url = isLiveStream ? video.VideoUrl : getProductionId(video.VideoUrl);
            return populateUrlsFromXml(video, getPlaylistDocument(url, !isLiveStream), isLiveStream);
        }
        #endregion

        #region Search
        public override bool CanSearch
        {
            get
            {
                return false;
            }
        }

        public override List<SearchResultItem> Search(string query, string category = null)
        {
            string html = GetWebData(string.Format("http://www.itv.com/itvplayer/search/term/{0}/catch-up", query));
            List<SearchResultItem> cats = new List<SearchResultItem>();
            foreach (Match match in searchRegex.Matches(html))
            {
                RssLink cat = new RssLink();
                cat.Thumb = match.Groups[2].Value;
                cat.Url = match.Groups[3].Value;
                cat.Name = cleanString(match.Groups[4].Value);
                cat.Description = cleanString(match.Groups[5].Value);
                cats.Add(cat);
            }

            return cats;
        }
        #endregion

        #region Shows
        int getShowsList(Category parentCategory)
        {
            List<Category> subCategories = new List<Category>();
            parentCategory.SubCategories = subCategories;
            parentCategory.SubCategoriesDiscovered = true;

            HtmlDocument document = GetWebData<HtmlDocument>((parentCategory as RssLink).Url);
            var showNodes = document.DocumentNode.SelectNodes(@"//a[@data-content-type='programme']");
            if (showNodes == null)
                return 0;

            foreach (var show in showNodes)
            {
                uint? episodeCount = null;
                var episodeCountNode = show.SelectSingleNode(@".//p[contains(@class, 'tout__meta') and not(time)]");
                if (episodeCountNode != null)
                {
                    episodeCount = parseNumber(episodeCountNode.InnerText);
                    if (!episodeCount.HasValue || episodeCount.Value == 0)
                        continue;
                }

                RssLink category = new RssLink();
                category.ParentCategory = parentCategory;
                category.EstimatedVideoCount = episodeCount;
                category.Url = show.GetAttributeValue("href", "");

                var titleNode = show.SelectSingleNode(@".//h3");
                if (titleNode != null)
                    category.Name = titleNode.GetCleanInnerText();

                var summaryNode = show.SelectSingleNode(@".//p[contains(@class, 'tout__summary')]");
                if (summaryNode != null)
                    category.Description = summaryNode.GetCleanInnerText();

                var imageScriptNode = show.SelectSingleNode(@".//script");
                if (imageScriptNode != null)
                    category.Thumb = getImageUrl(imageScriptNode.InnerText);

                subCategories.Add(category);
            }

            return subCategories.Count;
        }

        List<VideoInfo> getShowsVideos(Category category)
        {
            HtmlDocument document = GetWebData<HtmlDocument>((category as RssLink).Url);
            List<VideoInfo> videos = new List<VideoInfo>();

            addMultipleVideos(document, videos);
            if (videos.Count == 0)
                addSingleVideo(document, videos, category);

            return videos;
        }

        void addMultipleVideos(HtmlDocument document, List<VideoInfo> videos)
        {
            var episodeContainerNode = document.DocumentNode.SelectSingleNode(@"//div[@id='more-episodes']");
            if (episodeContainerNode == null)
                return;

            var episodeNodes = episodeContainerNode.SelectNodes(@".//a[@data-content-type='episode']");
            if (episodeNodes == null)
                return;

            foreach (var episode in episodeNodes)
            {
                VideoInfo video = new VideoInfo();
                video.VideoUrl = episode.GetAttributeValue("href", "");

                var titleNode = episode.SelectSingleNode(@".//h3");
                if (titleNode != null)
                    video.Title = titleNode.GetCleanInnerText();

                DateTime airDate;
                var airDateNode = episode.SelectSingleNode(@".//time");
                if (airDateNode != null &&
                    DateTime.TryParse(airDateNode.GetAttributeValue("datetime", ""), CultureInfo.InvariantCulture, DateTimeStyles.None, out airDate))
                    video.Airdate = airDate.ToString();

                var summaryNode = episode.SelectSingleNode(@".//p[contains(@class, 'tout__summary')]");
                if (summaryNode != null)
                    video.Description = summaryNode.GetCleanInnerText();

                var imageScriptNode = episode.SelectSingleNode(@".//script");
                if (imageScriptNode != null)
                    video.Thumb = getImageUrl(imageScriptNode.InnerText);
                videos.Add(video);
            }
        }

        void addSingleVideo(HtmlDocument document, List<VideoInfo> videos, Category category)
        {
            var videoInfoNode = document.DocumentNode.SelectSingleNode(@".//div[@id='episode-info']");
            if (videoInfoNode == null)
                return;

            VideoInfo video = new VideoInfo();
            video.VideoUrl = (category as RssLink).Url;
            video.Title = videoInfoNode.SelectSingleNode(@".//h1[@id='programme-title']").GetCleanInnerText();

            DateTime airDate;
            var airDateNode = videoInfoNode.SelectSingleNode(@".//li/time");
            if (airDateNode != null &&
                DateTime.TryParse(airDateNode.GetAttributeValue("datetime", ""), CultureInfo.InvariantCulture, DateTimeStyles.None, out airDate))
                video.Airdate = airDate.ToString();

            string description = "";
            var episodeTitleNode = videoInfoNode.SelectSingleNode(@".//h2[contains(@class, 'episode-info__episode-title')]");
            if (episodeTitleNode != null)
                description = string.Format("{0} - ", episodeTitleNode.GetCleanInnerText());
            var summaryNode = videoInfoNode.SelectSingleNode(@".//p[contains(@class, 'episode-info__synopsis')]");
            if (summaryNode != null)
                description += summaryNode.GetCleanInnerText();
            video.Description = description;

            var imageStyleNode = document.DocumentNode.SelectSingleNode(@"//style");
            if (imageStyleNode != null)
                video.Thumb = getImageUrl(imageStyleNode.InnerText);

            videos.Add(video);
        }
        #endregion

        #region Live Streams
        List<VideoInfo> getLiveStreams(Group group)
        {
            List<VideoInfo> vids = new List<VideoInfo>();
            foreach (Channel channel in group.Channels)
            {
                VideoInfo video = new VideoInfo();
                video.Title = channel.StreamName;
                video.Thumb = channel.Thumb;
                string url = channel.Url;
                string guideId;
                if (TVGuideGrabber.TryGetIdAndRemove(ref url, out guideId))
                {
                    NowNextDetails guide;
                    if (retrieveTVGuide && TVGuideGrabber.TryGetNowNext(guideId, out guide))
                        video.Description = guide.Format(tvGuideFormatString);
                }
                video.VideoUrl = url;
                vids.Add(video);
            }
            return vids;
        }
        #endregion

        #region Playlist Methods

        string populateUrlsFromXml(VideoInfo video, XmlDocument streamPlaylist, bool live)
        {
            if (streamPlaylist == null)
            {
                Log.Warn("ITVPlayer: Stream playlist is null");
                return "";
            }

            XmlNode videoEntry = streamPlaylist.SelectSingleNode("//VideoEntries/Video");
            if (videoEntry == null)
            {
                Log.Warn("ITVPlayer: Could not find video entry");
                return "";
            }

            XmlNode node;
            node = videoEntry.SelectSingleNode("./MediaFiles");
            if (node == null || node.Attributes["base"] == null)
            {
                Log.Warn("ITVPlayer: Could not find base url");
                return "";
            }

            string rtmpUrl = node.Attributes["base"].Value;
            SortedList<string, string> options = new SortedList<string, string>(new StreamComparer());
            foreach (XmlNode mediaFile in node.SelectNodes("./MediaFile"))
            {
                if (mediaFile.Attributes["delivery"] == null || mediaFile.Attributes["delivery"].Value != "Streaming")
                    continue;

                string title = "";
                if (mediaFile.Attributes["bitrate"] != null)
                {
                    title = mediaFile.Attributes["bitrate"].Value;
                    int bitrate;
                    if (int.TryParse(title, out bitrate))
                        title = string.Format("{0} kbps", bitrate / 1000);
                }

                if (!options.ContainsKey(title))
                {
                    string url = new MPUrlSourceFilter.RtmpUrl(rtmpUrl)
                    {
                        PlayPath = mediaFile.InnerText,
                        SwfUrl = "http://mediaplayer.itv.com/2.18.5%2Bbuild.ad408a9c67/ITVMediaPlayer.swf",
                        SwfVerify = true,
                        Live = live
                    }.ToString();
                    options.Add(title, url);
                }
            }

            if (RetrieveSubtitles)
            {
                node = videoEntry.SelectSingleNode("./ClosedCaptioningURIs");
                if (node != null && Helpers.UriUtils.IsValidUri(node.InnerText))
                    video.SubtitleText = SubtitleReader.TimedText2SRT(GetWebData(node.InnerText));
            }

            video.PlaybackOptions = new Dictionary<string, string>();
            if (options.Count == 0)
                return null;

            if (AutoSelectStream)
            {
                var last = options.Last();
                video.PlaybackOptions.Add(last.Key, last.Value);
            }
            else
            {
                foreach (KeyValuePair<string, string> key in options)
                    video.PlaybackOptions.Add(key.Key, key.Value);
            }
            return options.Last().Value;
        }

        string getProductionId(string url)
        {
            string html = GetWebData(url);
            Match m = productionIdRegex.Match(html);
            string productionId;
            if (m.Success)
            {
                productionId = m.Groups[1].Value.Replace("\\", "");
                Log.Debug("ITVPlayer: Found production id '{0}'", productionId);
            }
            else
            {
                productionId = "";
                Log.Warn("ITVPlayer: Failed to get production id");
            }
            return productionId;
        }

        string getPlaylist(string id, bool isProductionId = false)
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            if (isProductionId)
                doc.InnerXml = string.Format(SOAP_TEMPLATE, "<itv:ProductionId>" + id + "</itv:ProductionId>", "");
            else
                doc.InnerXml = string.Format(SOAP_TEMPLATE, "", id);

            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://mercury.itv.com/PlaylistService.svc");
                req.Headers.Add("SOAPAction", "http://tempuri.org/PlaylistService/GetPlaylist");
                req.Referer = "http://www.itv.com/mediaplayer/ITVMediaPlayer.swf?v=12.18.4/[[DYNAMIC]]/2";
                req.ContentType = "text/xml;charset=\"utf-8\"";
                req.Accept = "text/xml";
                req.Method = "POST";

                WebProxy proxy = getProxy();
                if (proxy != null)
                    req.Proxy = proxy;

                Stream stream;
                using (stream = req.GetRequestStream())
                    doc.Save(stream);

                using (stream = req.GetResponse().GetResponseStream())
                using (StreamReader sr = new StreamReader(stream))
                {
                    string responseXml = sr.ReadToEnd();
                    Log.Debug("ITVPlayer: Playlist response:\r\n\t {0}", responseXml);
                    return responseXml;
                }
            }
            catch (Exception ex)
            {
                Log.Warn("ITVPlayer: Failed to get playlist - {0}\r\n{1}", ex.Message, ex.StackTrace);
                return null;
            }
        }

        XmlDocument getPlaylistDocument(string id, bool isProductionId = false)
        {
            string xml = getPlaylist(id, isProductionId);
            if (!string.IsNullOrEmpty(xml))
            {
                XmlDocument document = new XmlDocument();
                try
                {
                    document.LoadXml(xml);
                    return document;
                }
                catch (Exception ex)
                {
                    Log.Warn("ITVPlayer: Failed to load plalist xml '{0}' - {1}\r\n{2}", xml, ex.Message, ex.StackTrace);
                }
            }
            return null;
        }
        #endregion

        #region Utils
        static string cleanString(string s)
        {
            s = stripTags(s);
            s = s.Replace("&amp;", "&").Replace("&pound;", "£").Replace("&#039;", "'");
            return Regex.Replace(s, @"\s\s+", " ").Trim();
        }

        static string stripTags(string s)
        {
            return Regex.Replace(s, "<[^>]*>", "");
        }

        static uint? parseNumber(string stringContainingNumber)
        {
            if (string.IsNullOrEmpty(stringContainingNumber))
                return null;

            uint result;
            Match m = numberRegex.Match(stringContainingNumber);
            if (m.Success && uint.TryParse(m.Value, out result))
                return result;
            return null;
        }

        string getImageUrl(string pictureNodeString)
        {
            if (string.IsNullOrEmpty(pictureNodeString))
                return "";

            Match m = imageRegex.Match(pictureNodeString);
            if (!m.Success)
            {
                m = singleVideoImageRegex.Match(pictureNodeString);
                if (!m.Success)
                    return "";
            }

            string imageUrl = HttpUtility.HtmlDecode(m.Groups[1].Value);
            if (!string.IsNullOrEmpty(thumbReplaceRegExPattern))
                imageUrl = Regex.Replace(imageUrl, thumbReplaceRegExPattern, thumbReplaceString);
            return imageUrl;
        }

        System.Net.WebProxy getProxy()
        {
            System.Net.WebProxy proxyObj = null;
            if (!string.IsNullOrEmpty(proxy))
            {
                proxyObj = new System.Net.WebProxy(proxy);
                if (!string.IsNullOrEmpty(proxyUsername) && !string.IsNullOrEmpty(proxyPassword))
                    proxyObj.Credentials = new System.Net.NetworkCredential(proxyUsername, proxyPassword);
            }
            return proxyObj;
        }
        #endregion
    }
}