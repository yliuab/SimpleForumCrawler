using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Models
{
    public class FixedFloorPost
    {
        public decimal Id { get; set; }
        public string Link { get; set; }
        public string Subject { get; set; }
        public string Floor1 { get; set; }
        public string Floor2 { get; set; }
        public string Floor3 { get; set; }
        public string Floor4 { get; set; }
        public string Floor5 { get; set; }
        public string Floor6 { get; set; }
        public string Floor7 { get; set; }
        public string Floor8 { get; set; }
        public string Floor9 { get; set; }
        public string Floor10 { get; set; }
        


        public FixedFloorPost()
        {
            Id = 0;
            Subject = "";
        }

        public FixedFloorPost(decimal id, string subject, string link)
        {
            Id = id;
            Subject = subject;
            Link = link;
        }
    }
}
