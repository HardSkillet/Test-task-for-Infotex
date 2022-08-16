using System;
using System.Xml;
using System.ServiceModel.Syndication;

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
                Html = "<!DOCTYPE html><html><head>";
                Html += "<title>" + feed.Generator + "</title>";
                Html += "<meta charset=utf-8 />";
                Html += "</head> <body>";
                Html += "<b><a href=\"Home/Set\">Настройки</a></b><br>";
                foreach (SyndicationItem item in feed.Items)
                {
                    Html += "<b>" + item.Title.Text + "</b>" + "<br>" + item.PublishDate.DateTime + "<br>" + item.Summary.Text + "<br><br>";
                }
                Html += "</body></html>";
            }
            return Html;
        }
    }
}