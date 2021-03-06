﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp1.Models;
using System.Text.RegularExpressions;

namespace WindowsFormsApp1.Logic
{
    public class Forum9GameRepository : CrawlerBase
    {
        public Forum9GameRepository() : base()
        {
            _pagesToCrawl = 10;

            DataFilePath = @"E:\Test\9GamePosts.csv";
            _urlPrefix = @"http://bbs.9game.cn/forum-4360-";

            _subjectRex = @"<a href\=.http://bbs.9game.cn/thread[-\d]*.html.*onclick\=.atarget\(this\).{1,3}class\=.xst.*</a>";
            _contentRex = @"id\=.postmessage_\d*.*([\r\n]*.*<br />*)*[\r\n]*.*</td>";
            _subjectSeparators = new string[] { "<ahref=", "style=", "onclick=atarget(this)class=xst>", "</a>" };
        }

        public override void SetUrlPrefix(string url)
        {
            _urlPrefix = url.Substring(0, url.Length - 6);
        }

        public override string GetCurrentUrl()
        {
            return _urlPrefix + _pageId + ".html";
        }

        public override Post GetPostkFromSubjectMatch(decimal postId, string matchText)
        {
            string filteredString = matchText.Replace(" ", "").Replace("\"", "");
            string[] splited = filteredString.Split(_subjectSeparators, StringSplitOptions.RemoveEmptyEntries);
            string link = splited[0].Trim();
            string subject = splited[splited.Length - 1].Trim();
            return new Post(postId, subject, link);
        }

    }
}
