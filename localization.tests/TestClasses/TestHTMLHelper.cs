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
        public List<(string Href, string Link)> GetNavLinks(string htmlContent)
        {
            List<(string, string)> result = new List<(string, string)>();

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            var ulNode = document.DocumentNode.Descendants()                            
                            .Where(n => n.Name == "ul" && n.GetClasses().Contains("nav") && n.GetClasses().Contains("navbar-nav"))
                            .FirstOrDefault();
            //HtmlNode ulNode = document.DocumentNode.SelectSingleNode("//ul[@class='nav navbar-nav']");

            var anchorNodes = ulNode.Descendants()                                            
                                            .Where(n => n.Name == "a");

            foreach (HtmlNode node in anchorNodes)
            {
                string href = node.GetAttributeValue("href", "");
                string value = node.InnerText;
                result.Add((href, value));
            }

            return result;
        }

        public Dictionary<string, string> GetInputsForForm(string htmlContent, bool includeHidden)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            var form = document.DocumentNode.Descendants()
                        .Where(n => n.Name == "form").FirstOrDefault();

            if (form == null)
            {
                return result;
            }

            var inputs = form.Descendants()
                            .Where(n => n.Name == "input");

            foreach (var input in inputs)
            {
                string name = input.GetAttributeValue("name", "");
                string value = input.GetAttributeValue("value", "");

                string type = input.GetAttributeValue("type", "");

                if (!includeHidden && type == "hidden")
                {
                    continue;
                }

                result.TryAdd(name, value);
            }

            return result;
        }

        public Dictionary<string, string> GetElements(string htmlContent, List<string> elementIds)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            foreach (string id in elementIds)
            {
                var element = document.GetElementbyId(id);
                if (element != null)
                {
                    result.TryAdd(id, element.InnerText);
                }
            }

            return result;
        }
    }
}
