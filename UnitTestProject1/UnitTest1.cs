using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsFormsApp1.Logic;
using System.Text.RegularExpressions;
using System;
using System.IO;
using System.Collections.Generic;
using CsvHelper;
using WindowsFormsApp1.Models;
using WindowsFormsApp1.Logic;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string text = "<a href=\"http://bbs.9game.cn/forum.php?mod=forumdisplay&fid=4360&amp;filter=typeid&amp;typeid=28253\">综合讨论</a>]</em> <a href=\"http://bbs.9game.cn/thread-31875452-1-9.html\" onclick=\"atarget(this)\" class=\"xst\" >888区 什么888区人满了？创建不了号?</a>";

            //List<Post> posts = new Forum9GameRepository().GetSubjectPostsFromWebText(text);
        }

        [TestMethod]
        public void TestMethod2()
        {
            string text = @"<table cellspacing='0' cellpadding='0'><tbody><tr><td class='t_f' id='postmessage_711903942'>
<br />
太史慈，孙坚，甘宁还可以勉强组一队</td></tr></tbody></table>";
            string rex = @"id\=.postmessage_\d*.*([\r\n]*.*<br />*)*[\r\n]*.*</td>";
            var links = Regex.Matches(text, rex);
            List<string> filtered = new List<string>();
            Forum9GameRepository repo = new Forum9GameRepository();
            foreach(Match m in links)
            {
                //filtered.Add(repo.FilterHtmlTags(m.Value));
            }
        }

        [TestMethod]
        public void TestMethod3()
        {
            string text = "http://bbs.nga.cn/thread.php?fid=538&page=2";
            
            string filtered = text.Substring(0, text.LastIndexOf('&'));

        }
    }
}
