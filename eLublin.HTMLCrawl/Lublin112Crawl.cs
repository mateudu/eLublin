using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using System.Net;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;

namespace eLublin.HTMLCrawl
{
    class Lublin112Crawl
    {
        public static async void Migrate()
        {
            var request = (HttpWebRequest)WebRequest.Create(@"http://www.lublin112.pl/");
            var response = request.GetResponse().GetResponseStream();
            var htmlText = new StreamReader(response).ReadToEnd();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlText);

            var nodes = doc.DocumentNode.SelectNodes("//script|//style");

            foreach (var node in nodes)
                node.ParentNode.RemoveChild(node);

            string htmlOutput = doc.DocumentNode.OuterHtml;
            //htmlOutput = htmlOutput.Replace("<!DOCTYPE html>", "");
            //Console.WriteLine(htmlOutput);
            //htmlText = HttpUtility.HtmlDecode(htmlText);

            //XmlDocument htmlDocument = new XmlDocument();
            //htmlDocument.Load(new MemoryStream(Encoding.UTF8.GetBytes(htmlOutput)));
            var d = doc.DocumentNode.Descendants().Where(x => x.Name == "h2" && x.ParentNode.ParentNode.GetAttributeValue("class", "") == "post");

            var _db = new Models.elublinDbEntities();
            string tempTytul, tempUrl;
            foreach (var elem in d)
            {
                //Console.WriteLine(elem.InnerHtml);
                //Console.WriteLine("==========================");

                try
                {
                    //XmlDocument htmlDocument = new XmlDocument();
                    //htmlDocument.Load(new MemoryStream(Encoding.UTF8.GetBytes(elem.InnerHtml)));
                    XDocument xdoc = XDocument.Parse(elem.InnerHtml);
                    xdoc.Root.Elements().Count();
                    //Console.WriteLine(xdoc.Descendants());
                    foreach (var el2 in xdoc.Descendants())
                    {
                        //tempTytul = el2.Value.Substring(1,el2.Value.Length-1);
                        tempTytul=Regex.Replace(el2.Value, @"\t|\n|\r", "");                        
                        tempTytul = el2.Value.Trim(' ', '	');
                        tempUrl = el2.Attribute("href").Value;
                        if (!_db.Notifications.Any(x=>x.tytul==tempTytul && x.url== tempUrl))
                        {                            
                            _db.Notifications.Add(new Models.Notification
                            {
                                czasDodania =DateTime.Now,
                                tytul= tempTytul,
                                url= tempUrl
                            });
                        }
                        //_db.Notifications.Add
                        //Console.WriteLine(el2.Attribute("href").Value);
                        //Console.WriteLine(el2.Value);
                    }
                    //htmlDocument.Load(new MemoryStream(Encoding.UTF8.GetBytes(elem.InnerHtml)));
                    //string title = htmlDocument.
                    //Console.WriteLine(title);
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                    //con
                }
            }
            try
            {
                await _db.SaveChangesAsync();
            }
            catch(Exception exc)
            {
                Console.WriteLine(DateTime.Now.ToString() + " Lublin112 EXC: " + exc.Message);
            }
            //Console.WriteLine(d.Count());

            //Console.WriteLine(htmlDocument.InnerXml);
            //Console.WriteLine(HttpUtility.HtmlDecode(htmlOutput ?? " "));

            //var xdoc = XDocument.Parse(htmlText);

            //Console.WriteLine(htmlText);

            // Select the body tag
            //var bodyNode = htmlDocument.GetElementsByTagName("body").Item(0);

            // Modify the contents of the body
            //bodyNode.InnerText = "Hello";

            //Console.WriteLine(htmlDocument.InnerXml);
        }
    }
}
