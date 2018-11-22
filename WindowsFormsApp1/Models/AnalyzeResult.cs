
namespace WindowsFormsApp1.Models
{
    public class AnalyzeResult
    {
        public decimal Id { get; set; }
        public string Keywords2 { get; set; }
        public int Keywords2Count { get; set; }

        public string Keywords3 { get; set; }
        public int Keywords3Count { get; set; }

        public string Keywords4 { get; set; }
        public int Keywords4Count { get; set; }

        public AnalyzeResult()
        {
            Id = 0;
        }
    }
}
