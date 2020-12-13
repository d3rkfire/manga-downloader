using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace Manga_Downloader.Classes
{
    class Parser
    {
        public static Series ParseSeries(string link)
        {
            Console.WriteLine(Regex.Match(link, @"https://mangapark.net/manga/.*").Success);
            if (Regex.Match(link, @"https://bato.to/series/\d*").Success) return ParseSeriesBatoto(link);
            else if (Regex.Match(link, @"https://mangapark.net/manga/.*").Success) return ParseSeriesMangapark(link);
            else if (Regex.Match(link, @"https://toonily.com/webtoon/.*").Success) return ParseSeriesToonily(link);
            else throw new Exception("Unsupported link.");
        }
        public static Chapter ParseChapter(Chapter chapter)
        {
            // Parse Chapter Images
            if (Regex.Match(chapter.Link, @"https://bato.to/chapter/\d*").Success) return ParseChapterBatoto(chapter);
            else if (Regex.Match(chapter.Link, @"https://mangapark.net/manga/.*/i\d*").Success) return ParseChapterMangapark(chapter);
            else if (Regex.Match(chapter.Link, @"https://toonily.com/webtoon/.*/chapter-\d*/").Success) return ParseChapterToonily(chapter);
            else throw new Exception("Unsupported link.");
        }

        static Series ParseSeriesBatoto(string link)
        {
            // Variables to be retrieved
            string title;
            string cover;
            ObservableCollection<Chapter> chapters = new ObservableCollection<Chapter>();

            // Neccessary variables
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(link);

            // Get Title
            HtmlNode titleNode = doc.DocumentNode.SelectSingleNode("//h3[@class='item-title']");
            title = titleNode.InnerText.Trim();
            foreach (char ch in System.IO.Path.GetInvalidFileNameChars()) title = title.Replace(ch, '_');

            // Get Cover
            HtmlNode coverNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'attr-cover')]/img");
            cover = "https:" + coverNode.Attributes["src"].Value;

            // Get Chapters
            HtmlNode[] nodes = doc.DocumentNode.SelectNodes("//a[@class='chapt']").ToArray();
            for (int i = nodes.Length - 1; i >= 0; i--)
            {
                // Chapter Title
                string chapterTitle = nodes[i].ChildNodes[1].InnerText.Trim();
                foreach (char ch in System.IO.Path.GetInvalidFileNameChars()) chapterTitle = chapterTitle.Replace(ch, '_');

                // Chapter Link
                string chapterLink = "https://bato.to" + nodes[i].Attributes["href"].Value;
                chapters.Add(new Chapter(chapterTitle, chapterLink));
            }
            return new Series(title, cover, chapters);
        }
        static Chapter ParseChapterBatoto(Chapter chapter)
        {
            chapter.Images.Clear();
            chapter.Status = Chapter.ProgressStatus.PARSING;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(chapter.Link);
            HtmlNodeCollection scriptNodes = doc.DocumentNode.SelectNodes("//script");
            for (int j = 0; j < scriptNodes.Count; j++)
                if (scriptNodes[j].InnerHtml.Contains("images"))
                {
                    MatchCollection mc = Regex.Matches(scriptNodes[j].InnerHtml, "https://[^\"]*");
                    foreach (Match m in mc) chapter.Images.Add(m.Value);
                    break;
                }
            chapter.Status = Chapter.ProgressStatus.PARSED;
            return chapter;
        }

        static Series ParseSeriesMangapark(string link)
        {
            // Variables to be retrieved
            string title;
            string cover;
            ObservableCollection<Chapter> chapters = new ObservableCollection<Chapter>();

            // Neccessary variables
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(link);

            // Get Cover
            HtmlNode coverNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'cover')]/img");
            cover = "https:" + coverNode.Attributes["src"].Value;
            
            // Get Title
            title = coverNode.Attributes["title"].Value.Trim();
            foreach (char ch in System.IO.Path.GetInvalidFileNameChars()) title = title.Replace(ch, '_');

            // Get Chapters
            HtmlNode[] nodes = doc.DocumentNode.SelectNodes("//ul[@class='chapter']/li/div/a[contains(@class, 'ch')]").ToArray();
            for (int i = nodes.Length - 1; i >= 0; i--)
            {
                // Chapter Link
                string chapterLink = "https://mangapark.net" + nodes[i].Attributes["href"].Value;
                chapterLink = Regex.Replace(chapterLink, @"(.*i\d*).*", "$1");

                // Chapter Title
                string chapterTitle = nodes[i].InnerText.Trim();
                foreach (char ch in System.IO.Path.GetInvalidFileNameChars()) chapterTitle = chapterTitle.Replace(ch, '_');

                chapters.Add(new Chapter(chapterTitle, chapterLink));
            }
            return new Series(title, cover, chapters);
        }
        static Chapter ParseChapterMangapark(Chapter chapter)
        {
            chapter.Images.Clear();
            chapter.Status = Chapter.ProgressStatus.PARSING;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(chapter.Link);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//script");
            for (int i = 0; i < nodes.Count; i++)
                if (nodes[i].InnerHtml.Contains("_load_pages"))
                {
                    MatchCollection mc = Regex.Matches(nodes[i].InnerHtml, "http[^\"]*");
                    foreach (Match m in mc) chapter.Images.Add(m.Value.Replace("\\", ""));
                    break;
                }
            chapter.Status = Chapter.ProgressStatus.PARSED;
            return chapter;
        }

        static Series ParseSeriesToonily(string link)
        {
            // Variables to be retrieved
            string title;
            string cover;
            ObservableCollection<Chapter> chapters = new ObservableCollection<Chapter>();

            // Neccessary variables
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(link);

            // Get Title
            HtmlNode titleNode = doc.DocumentNode.SelectSingleNode("//span[@class='rate-title']");
            title = titleNode.InnerText.Trim();
            foreach (char ch in System.IO.Path.GetInvalidFileNameChars()) title = title.Replace(ch, '_');

            // Get Cover
            HtmlNode coverNode = doc.DocumentNode.SelectSingleNode("//div[@class='summary_image']/a/img");
            cover = coverNode.Attributes["data-src"].Value;

            // Get Chapters
            HtmlNode[] nodes = doc.DocumentNode.SelectNodes("//li[contains(@class, 'wp-manga-chapter')]/a").ToArray();
            for (int i = nodes.Length - 1; i >= 0; i--)
            {
                // Chapter Title
                string chapterTitle = nodes[i].InnerText.Trim();
                foreach (char ch in System.IO.Path.GetInvalidFileNameChars()) chapterTitle = chapterTitle.Replace(ch, '_');

                // Chapter Link
                string chapterLink = nodes[i].Attributes["href"].Value;
                chapters.Add(new Chapter(chapterTitle, chapterLink));
            }
            return new Series(title, cover, chapters);
        }

        static Chapter ParseChapterToonily(Chapter chapter)
        {
            chapter.Images.Clear();
            chapter.Status = Chapter.ProgressStatus.PARSING;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(chapter.Link);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//img[contains(@class, 'wp-manga-chapter-img')]");
            for (int i = 0; i < nodes.Count; i++) chapter.Images.Add(nodes[i].Attributes["data-src"].Value.Trim());
            chapter.Status = Chapter.ProgressStatus.PARSED;
            return chapter;
        }
    }
}
