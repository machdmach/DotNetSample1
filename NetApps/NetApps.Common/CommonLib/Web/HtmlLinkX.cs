using System.Net;
using System.Text.RegularExpressions;

namespace System
{
    public class HmlLinkX { }
    public class HyperLinkX
    {
        public string HRef { get; set; }
        public string Target { get; set; }
        public string Text { get; set; }
        public string ToHtml(string baseUrl)
        {
            var uri = new Uri(baseUrl);
            string authority = uri.Authority;

            //string rval = string.Format("Href={0}, Text={1}, Target={2}", Href, HtmlText.HtmlEncode(Text), Target);
            string absoluteUrl;
            string hrefLower = HRef.ToLower();
            if (hrefLower.StartsWith("http:") || hrefLower.StartsWith("https:"))
            {
                absoluteUrl = HRef;
            }
            else
            {
                if (HRef.StartsWith("/"))
                {
                    absoluteUrl = authority + HRef;
                }
                else
                {
                    absoluteUrl = baseUrl + HRef;
                }
                //absoluteUrl = VirtualPathUtility.Combine(baseUrl, Href);
            }

            string link = string.Format("<a href='{0}'> View </a>", absoluteUrl);
            string linkText = string.Format("<a href='{0}'>{1}</a>", HRef, Text);
            string rval = System.Net.WebUtility.HtmlEncode(linkText) + " " + link;
            return rval;
        }

        public static void GetScript_AddSpExploreLinks()
        {
            string serverRelativeUrl = "z";

            serverRelativeUrl = "/public_defender";
            serverRelativeUrl = "/ccgis";

            string url = "http://www.clarkcountynv.gov/depts" + serverRelativeUrl; ///public_defender"; ///pages/default.aspx";
            string s = "";
            //            s = @"
            //             ";

            var wc = new WebClient();
            s = wc.DownloadString(url);

            var links = ParseLinks(s); //, fromText, toText);
            var buf = new StringBuilder();
            buf.AppendFormat("ExploreLinks--Init '{0}'\n", serverRelativeUrl.Replace("_", "-"));

            foreach (var link in links)
            {
                string href = link.HRef;
                if (href.ToLower().StartsWith("http://staging-internet") ||
                    href.ToLower().StartsWith("http://clarkcountynv.gov") ||
                    href.ToLower().StartsWith("http://www.clarkcountynv.gov"))
                {
                    Uri uri = new Uri(link.HRef);
                    href = uri.PathAndQuery;
                }
                buf.AppendFormat("ListItem--Add-HyperLink $g_ExploreLinks '{0}' '{1}'\n", link.Text, href);
            }
            MOutput.Write_TextArea(buf);

            MOutput.WriteHtmlTable(url, links);
        }
        //===================================================================================================
        public static HyperLinkX ParseLink(String htmlText)
        {
            var links = ParseLinks(htmlText);
            if (links.Count > 0)
            {
                return links[0];
            }
            else
            {
                return null;
            }
        }
        //===================================================================================================
        public static List<HyperLinkX> ParseLinks(String htmlText)
        {
            htmlText = htmlText.Replace('"', '\'');
            var rval = new List<HyperLinkX>();
            /*
            string s = htmlText;

            s = s.Replace('"', '\'');

            int i1 = s.IndexOf(fromText);
            if (i1 < 0)
            {
                return rval;
            }
            int i2 = s.IndexOf(toText, i1 + 3);
            if (i2 < 0)
            {
                throw new Exception("Cant find toText: " + toText);
            }
            s = s.Substring(i1, i2 - i1);
            */

            //int a1 = s.IndexOf("<a ");
            //while (a1 >= 0)
            //{
            //    int a2 = s.IndexOf("</a>", a1);
            //    string a = s.Substring(a1, a2 - a1);
            //    a += "</a>";
            //    s = s.Substring(a2);
            //}

            //<a\s+(?:[^>]*?\s+)?href="([^"]*)" 
            //<a\s+(?:[^>]*?\s+)?href="([^"]+\?[^"]+)"
            MatchCollection m1 = Regex.Matches(htmlText, @"(<a.*?>.*?</a>)", RegexOptions.Singleline);
            foreach (Match m in m1)
            {
                string aElt = m.Groups[1].Value;
                var hl = new HyperLinkX();

                // 3.
                // Get href attribute.
                Match m2 = Regex.Match(aElt, @"href='(.*?)'", RegexOptions.Singleline);
                if (m2.Success)
                {
                    hl.HRef = m2.Groups[1].Value;
                }
                // 3.
                // Get target attribute.
                Match targetM = Regex.Match(aElt, @"target='(.*?)'", RegexOptions.Singleline);
                if (targetM.Success)
                {
                    hl.Target = targetM.Groups[1].Value;
                }

                // 4.
                // Remove inner tags from text.
                string t = Regex.Replace(aElt, @"\s*<.*?>\s*", "", RegexOptions.Singleline);
                hl.Text = t;
                rval.Add(hl);
            }
            return rval;
        }
        //===================================================================================================
        public static List<string> GetLinks2(string url, String htmlText)
        {
            //@<a\s*href\s*=\s*['"]?([^'"]*)[^>]*>([^<]*)
            //@"<A[^>]*?HREF\s*=\s*[""']?([^'"" >]+?)[ '""]?>";

            string pattern = @"<a\s*href\s*=\s*['""]?([^'""]*)[^>]*>([^<]*)";
            //                <A[^>]*?HREF\s*=\s*["']?([^'" >]+?)[ '"]?>
            // return Regex.Replace(htmlText, pattern, " ");

            MatchCollection mc = Regex.Matches(htmlText, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            List<String> rval = new List<string>();
            foreach (Match m in mc)
            {
                //Log.Debug("Match=" + m);
                //Log.Debug("Group1=" + m.Groups[1]);
                string url2 = m.Groups[0].Value;
                string urlText2 = m.Groups[1].Value;
                rval.Add(url2);
            }

            return rval;

            /*
            String s = "dd";

            // find links in simple html
            string t18 = @"
   <html>
   <a href=""first.htm"">first tag text</a>
   <a href=""next.htm"">next tag text</a>
   </html>
   ";
            string pattern = @"<[^>]*>";
            s = Regex.Replace(s, pattern, " ");

            MatchCollection mc = Regex.Matches(s, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            StringBuilder buf = new StringBuilder();
            foreach (Match m in mc)
            {
               //log.Debug("Match=" + m);
               //log.Debug("Group1=" + m.Groups[1]);
            }
            return buf.ToString();
             * */
        }
    }
}
