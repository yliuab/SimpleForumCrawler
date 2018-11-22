using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.Models;
using System.Text.RegularExpressions;

namespace WindowsFormsApp1.Logic
{
    public class NGARepository : CrawlerBase
    {
        public NGARepository(): base()
        {
            _pagesToCrawl = 2;

            DataFilePath = @"E:\Test\NGAPosts.csv";
            _urlPrefix = @"http://bbs.nga.cn/thread.php?fid=538";

            _subjectRex = @"<a href\='/read.php.tid\=\d*'.*class='topic'.*</a>";
            _contentRex = @"<[span]* id\=.postcontent\d*.*class\=.postcontent.*</[span]*>";
            _subjectSeparators = new string[] { "<ahref=", "id=", "class=topic>", "</a>" };
        }

        public override void SetUrlPrefix(string url)
        {
            if (url.Contains("&page="))
            {
                _urlPrefix = url.Substring(0, url.LastIndexOf('&'));
            }
            else
            {
                _urlPrefix = url;
            }
        }

        public override string GetCurrentUrl()
        {
            if (_pageId == 1)
            {
                return _urlPrefix;
            }
            return _urlPrefix + "&page=" + _pageId;
        }

        public override Post GetPostkFromSubjectMatch(decimal postId, string matchText)
        {
            string filteredString = matchText.Replace(" ", "").Replace("\"", "").Replace("'","");
            string[] splited = filteredString.Split(_subjectSeparators, StringSplitOptions.RemoveEmptyEntries);
            string link = "http://bbs.nga.cn" + splited[0].Trim();
            string subject = splited[splited.Length - 1].Trim();
            return new Post(postId, subject, link);
        }

    }
}
