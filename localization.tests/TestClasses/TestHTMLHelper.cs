using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace localization.tests.TestClasses
{
    class TestHTMLHelper
    {
        public List<(string Href, string Link)> GetNavLinks(string a_htmlContent)
        {
            List<(string, string)> result = new List<(string, string)>();

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(a_htmlContent);

            var ulNode = document.DocumentNode.Descendants()
                            .Where(n => n.NodeType == HtmlNodeType.Element)
                            .Where(n => n.Name == "ul" && n.GetClasses().Contains("nav") && n.GetClasses().Contains("navbar-nav"))
                            .FirstOrDefault();
            //HtmlNode ulNode = document.DocumentNode.SelectSingleNode("//ul[@class='nav navbar-nav']");

            var anchorNodes = ulNode.Descendants()
                                            .Where(n => n.NodeType == HtmlNodeType.Element)
                                            .Where(n => n.Name == "a");

            foreach (HtmlNode node in anchorNodes)
            {
                string href = node.GetAttributeValue("href", "");
                string value = node.InnerText;
                result.Add((href, value));
            }

            return result;
        }        
    }
}
