using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Models
{
    public class Post
    {
        public decimal Id { get; set; }
        public string Link { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public List<string> Floors;

        private char _separator = '|';

        public Post()
        {
            Id = 0;
            Subject = "";
            Floors = new List<string>();
        }

        public Post(decimal id, string subject, string link)
        {
            Id = id;
            Subject = subject;
            Link = link;
            Floors = new List<string>();
        }

        public void SetContentFromFloors()
        {
            Content = string.Join(_separator.ToString(), Floors);
        }

        public void SetFloorsFromContent()
        {
            Floors = Content.Split(_separator).ToList();
        }
    }
}
