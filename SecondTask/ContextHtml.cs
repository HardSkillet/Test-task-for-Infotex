using System;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Text;

namespace SecondTask {
    public class ContextHtml {
        public string Html { get; set; }
        public DateTime Time { get; set; }
        private bool isExist = false;
        public String Rss(ConfigRss config, bool flag) {
            if (!isExist)
            {
                isExist = true;
                Time = DateTime.Now;
            }
            if (Time < DateTime.Now.AddMinutes((int)config.Time) || flag) {
                XmlTextReader reader = new XmlTextReader(config.Url);
                SyndicationFeed feed = SyndicationFeed.Load(reader);
                var sb = new StringBuilder();
                sb.Append("<!DOCTYPE html><html><head>");
                sb.Append("<title>" + feed.Generator + "</title>");
                sb.Append("<meta charset=utf-8 />");
                sb.Append("</head> <body>");
                sb.Append("<b><a href=\"Home/Set\">Настройки</a></b><br>");
                foreach (SyndicationItem item in feed.Items)
                {
                    sb.Append("<b>" + item.Title.Text + "</b>" + "<br>" + item.PublishDate.DateTime + "<br>" + item.Summary.Text + "<br><br>");
                }
                sb.Append("</body></html>");
                Html = sb.ToString();
            }
            return Html;
        }
    }
}