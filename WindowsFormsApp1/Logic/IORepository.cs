using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Logic
{
    class IORepository
    {
        public IORepository()
        {
        }

        public void WriteCsvFile<T>(List<T> content, string path)
        {
            var csv = new CsvWriter(new StreamWriter(path, false, Encoding.UTF8));
            csv.WriteRecords(content);
            csv.Flush();
            csv.Dispose();
        }

        public List<Post> ReadCsvFile(string path)
        {
            var csv = new CsvReader(new StreamReader(path, Encoding.UTF8));
            List<Post> titles = csv.GetRecords<Post>().ToList();
            return titles;
        }
    }
}
