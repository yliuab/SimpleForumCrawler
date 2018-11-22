using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.Models;
using System.Text.RegularExpressions;

namespace WindowsFormsApp1.Logic
{
    public class BaiduTiebaRepository : CrawlerBase
    {
        public BaiduTiebaRepository() : base()
        {
            _pagesToCrawl = 6;

            DataFilePath = @"E:\Test\BaiduTiebaPosts.csv";
            _urlPrefix = @"https://tieba.baidu.com/f?kw=%E7%8E%87%E5%9C%9F%E4%B9%8B%E6%BB%A8&ie=utf-8&pn=";

            _subjectRex = @"<a.rel\=.*href\=./p/\d*.*class\=.j_th_tit.*</a>";
            _contentRex = @"<div id\=.post_content_\d*. class\=.*style\=.*>.*</div><br>";
            _subjectSeparators = new string[] { "href=", "title=", "class=j_th_tit>", "</a>" };
        }

        public override void SetUrlPrefix(string url)
        {
            if (url.Contains("&pn="))
            {
                _urlPrefix = url.Substring(0, url.Length - 1);
            }
            else
            {
                _urlPrefix = url + "&pn=";
            }
        }

        public override string GetCurrentUrl()
        {
            return _urlPrefix + Math.Max(2, ((_pageId - 1) * 50 + 2)).ToString();
        }

        public override Post GetPostkFromSubjectMatch(decimal postId, string matchText)
        {
            string filteredString = matchText.Replace(" ", "").Replace("\"", "");
            string[] splited = filteredString.Split(_subjectSeparators, StringSplitOptions.RemoveEmptyEntries);
            string link = "https://tieba.baidu.com" + splited[1].Trim();
            string subject = splited[3].Trim();
            return new Post(postId, subject, link);
        }

    }
}
