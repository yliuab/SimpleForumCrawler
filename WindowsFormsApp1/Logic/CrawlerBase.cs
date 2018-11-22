using System;
using System.Collections.Generic;
using System.Linq;
using WindowsFormsApp1.Models;
using System.Text.RegularExpressions;

namespace WindowsFormsApp1.Logic
{
    public abstract class CrawlerBase
    {
        public int Progress
        {
            get
            {
                if (_isCompleted)
                {
                    return 100;
                }
                decimal postToGo = _allPosts.Count - _completedPosts;
                decimal completedPages = postToGo == 0 ? _completedPages : _completedPages + ((_postPerPage - postToGo) / _postPerPage);
                decimal percentage = (completedPages / _pagesToCrawl) * 100;

                //return values from 0-99
                return Math.Min(99, Math.Max(0, (int)Math.Round(percentage)));
            }
        }

        public string DataFilePath;

        protected int _pagesToCrawl;
        protected int _postPerPage;
        protected string _urlPrefix;

        protected string _subjectRex;
        protected string _contentRex;
        protected string[] _subjectSeparators;

        protected decimal _pageId;
        protected decimal _postId;
        protected List<Post> _allPosts;

        protected decimal _completedPages = 0;
        protected decimal _completedPosts = 0;

        protected bool _isCompleted = false;

        public CrawlerBase()
        {
            _pagesToCrawl = 10;
            _postPerPage = 10;

            _completedPages = 0;
            _completedPosts = 0;
        }

        public abstract void SetUrlPrefix(string url);

        public abstract string GetCurrentUrl();

        public abstract Post GetPostkFromSubjectMatch(decimal postId, string matchText);

        public virtual void StartCrawl(string targetUrl = "")
        {
            if (!string.IsNullOrWhiteSpace(targetUrl))
            {
                SetUrlPrefix(targetUrl);
            }
            _allPosts = new List<Post>();
            _postId = 0;
            _completedPages = 0;
            _completedPosts = 0;
            _pageId = 1;
            Console.WriteLine("============ Start crawling page " + _pageId);
            _isCompleted = false;
            new AbotRepository().CrawWebPage(GetCurrentUrl(), new OnCompleteCallbackDelegate(this.OnCompleteSubjectsCallback), _pageId);
        }

        public virtual void OnCompleteSubjectsCallback(decimal pageId, string webContentText)
        {
            List<Post> subjectPosts = GetSubjectPostsFromWebText(webContentText);
            _allPosts.AddRange(subjectPosts);
            _postPerPage = subjectPosts.Count;

            foreach (Post p in subjectPosts)
            {
                if (!string.IsNullOrWhiteSpace(p.Link))
                {
                    new AbotRepository().CrawWebPage(p.Link, new OnCompleteCallbackDelegate(this.OnCompleteContentsCallback), p.Id);
                }
            }
        }

        public virtual List<Post> GetSubjectPostsFromWebText(string webContentText)
        {
            List<Post> posts = new List<Post>();
            var links = Regex.Matches(webContentText, _subjectRex);
            if (links.Count == 0)
            {
                Console.WriteLine("============No Match");
            }

            foreach (Match m in links)
            {
                _postId++;
                Console.WriteLine("============" + m.Value);
                posts.Add(GetPostkFromSubjectMatch(_postId, m.Value));
            }
            return posts;
        }

        public virtual List<string> GetContentsFromWebText(string webContentText)
        {
            List<string> contents = new List<string>();
            var links = Regex.Matches(webContentText, _contentRex);
            if (links.Count == 0)
            {
                Console.WriteLine("============No Match");
            }

            foreach (Match m in links)
            {
                string filtered = FilterHtmlTags(m.Value).Trim();
                Console.WriteLine("============" + filtered);
                contents.Add(filtered);
            }
            return contents;
        }

        public virtual void OnCompleteContentsCallback(decimal postId, string webContentText)
        {
            Post post = _allPosts.FirstOrDefault(p => p.Id == postId);
            List<string> contents = GetContentsFromWebText(webContentText);
            for (int i = 0; i < contents.Count; i++)
            {
                post.Floors.Add(contents.ElementAt(i));
            }
            post.SetContentFromFloors();
            _completedPosts++;
            if (_completedPosts == _allPosts.Count)
            {
                _completedPages++;
                if (_completedPages >= _pagesToCrawl)
                {
                    Console.WriteLine("============ Crawling Forum completed");
                    PostCompleteTask();
                }
                else
                {
                    // Crawl next page
                    _pageId++;
                    Console.WriteLine("============ Start crawling page " + _pageId);
                    new AbotRepository().CrawWebPage(GetCurrentUrl(), new OnCompleteCallbackDelegate(this.OnCompleteSubjectsCallback), _pageId);
                }
            }
        }

        public virtual void PostCompleteTask()
        {
            WritePostsToFile(DataFilePath);
            _isCompleted = true;
        }

        public virtual void WritePostsToFile(string path)
        {
            Console.WriteLine("============ Start writing posts data to file: " + path);
            new IORepository().WriteCsvFile(_allPosts, path);
            Console.WriteLine("============ Writing file completed");
        }

        public virtual string FilterHtmlTags(string webText)
        {
            string filtered = "";
            string[] splited = webText.Split('<');
            for (int i = 0; i < splited.Length; i++)
            {
                if (splited[i].Contains(">"))
                {
                    string[] second = splited[i].Split('>');
                    if (second.Length > 1 && !string.IsNullOrWhiteSpace(second[1]))
                    {
                        filtered += second[1];
                    }
                }
                else if (!string.IsNullOrWhiteSpace(splited[i]))
                {
                    filtered += splited[i];
                }
            }
            filtered = filtered.Replace("&nbsp;", " ");//.Replace("\r\n", "");
            if (filtered.StartsWith("\n"))
            {
                filtered = filtered.Substring(1);
            }
            return filtered;
        }

    }
}
