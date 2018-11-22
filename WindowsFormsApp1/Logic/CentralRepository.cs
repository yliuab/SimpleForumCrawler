using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Logic
{
    public delegate void OnCompleteCallbackDelegate(decimal Id, string webContentText);

    public class CentralRepository
    {
        public Forum SelectedForum;

        public int Progress
        {
            get
            {
                return _selectedRepository.Progress;
            }
        }

        public string DataFilePath
        {
            get
            {
                return _selectedRepository.DataFilePath;
            }
            set
            {
                _selectedRepository.DataFilePath = value;
            }
        }

        private Dictionary<Forum, CrawlerBase> _repositories;
        private CrawlerBase _selectedRepository
        {
            get
            {
                return _repositories[SelectedForum];
            }
        }

        public CentralRepository()
        {
            _repositories = new Dictionary<Forum, CrawlerBase>();
            _repositories.Add(Forum.Forum163, new Forum163Repository());
            _repositories.Add(Forum.Forum9Game, new Forum9GameRepository());
            _repositories.Add(Forum.BaiduTieba, new BaiduTiebaRepository());
            _repositories.Add(Forum.NGA, new NGARepository());
        }

        public void StartCrawlSelectedForum(string url = "")
        {

            _selectedRepository.StartCrawl(url);
        }

        public void AnalyzeSelectedForum()
        {
            AnalyzeKeywords(_selectedRepository.DataFilePath);
        }

        /// <summary>
        /// Get keywords that contains 2-4 characters
        /// </summary>
        /// <param name="dataPath"></param>
        private void AnalyzeKeywords(string dataPath)
        {
            List<Post> posts = new IORepository().ReadCsvFile(dataPath);
            Dictionary<string, int> keyCount2 = new Dictionary<string, int>();
            Dictionary<string, int> keyCount3 = new Dictionary<string, int>();
            Dictionary<string, int> keyCount4 = new Dictionary<string, int>();
            foreach (Post p in posts)
            {
                p.SetFloorsFromContent();
                keyCount2 = this.AnalyzeKeywordsFromText(keyCount2, p.Subject, 2);
                keyCount3 = this.AnalyzeKeywordsFromText(keyCount3, p.Subject, 3);
                keyCount4 = this.AnalyzeKeywordsFromText(keyCount4, p.Subject, 4);
                foreach (string f in p.Floors)
                {
                    keyCount2 = this.AnalyzeKeywordsFromText(keyCount2, f, 2);
                    keyCount3 = this.AnalyzeKeywordsFromText(keyCount3, f, 3);
                    keyCount4 = this.AnalyzeKeywordsFromText(keyCount4, f, 4);
                }
            }

            List<KeyValuePair<string, int>> keyCount2List = keyCount2.OrderByDescending(kp => kp.Value).ToList();
            List<KeyValuePair<string, int>> keyCount3List = keyCount3.OrderByDescending(kp => kp.Value).ToList();
            List<KeyValuePair<string, int>> keyCount4List = keyCount4.OrderByDescending(kp => kp.Value).ToList();

            int count = 0;
            //string hotKeys = "";
            int max = Math.Max(Math.Max(keyCount2List.Count, keyCount3List.Count), keyCount4List.Count);
            List<AnalyzeResult> results = new List<AnalyzeResult>();
            for (int i = 0; i < max; i++)
            {
                AnalyzeResult r = new AnalyzeResult();
                r.Id = i;
                count++;
                if (keyCount2List.Count > i + 1)
                {
                    KeyValuePair<string, int> kp2 = keyCount2List.ElementAt(i);
                    //hotKeys += kp2.Key + " (" + kp2.Value + ")          ";
                    r.Keywords2 = kp2.Key;
                    r.Keywords2Count = kp2.Value;
                }
                if (keyCount3List.Count > i + 1)
                {
                    KeyValuePair<string, int> kp3 = keyCount3List.ElementAt(i);
                    //hotKeys += kp3.Key + " (" + kp3.Value + ")          ";
                    r.Keywords3 = kp3.Key;
                    r.Keywords3Count = kp3.Value;
                }
                if (keyCount4List.Count > i + 1)
                {
                    KeyValuePair<string, int> kp4 = keyCount4List.ElementAt(i);
                    //hotKeys += kp4.Key + " (" + kp4.Value + ")          ";
                    r.Keywords4 = kp4.Key;
                    r.Keywords4Count = kp4.Value;
                }
                //hotKeys += "\n";
                results.Add(r);
                if (count == 200)
                {
                    break;
                }
            }
            new IORepository().WriteCsvFile(results, @"E:\Test\AnalyzeResults" + SelectedForum.ToString() + ".csv");
            //MessageBox.Show(hotKeys);
        }

        private Dictionary<string, int> AnalyzeKeywordsFromText(Dictionary<string, int> keyCount, string text, int fixWordNum = 0)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return keyCount;
            }
            var sentence = Regex.Matches(text, @"[\u4e00-\u9fa5a-zA-Z0-9]*");
            foreach (Match m in sentence)
            {
                // 2 words - 4 words
                for (int i = 2; i < 5; i++)
                {
                    int wordNum = fixWordNum > 0 ? fixWordNum : i;
                    for (int j = 0; j < m.Value.Length - 1; j++)
                    {
                        if (wordNum + j > m.Value.Length)
                        {
                            break;
                        }
                        string sub = m.Value.Substring(j, wordNum);
                        if (keyCount.ContainsKey(sub))
                        {
                            keyCount[sub]++;
                        }
                        else
                        {
                            keyCount.Add(sub, 1);
                        }
                    }
                }
            }
            return keyCount;
        }
    }
}
