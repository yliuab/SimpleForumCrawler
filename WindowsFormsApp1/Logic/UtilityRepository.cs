using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1.Logic
{
    public class UtilityRepository
    {
        public UtilityRepository()
        {

        }

        public string GetFormatTimeString(int seconds)
        {
            int sec = seconds % 60;
            int min = seconds / 60;
            int hour = min / 60;
            return string.Format("{0}:{1}:{2}", hour.ToString("00"), min.ToString("00"), sec.ToString("00"));
        }
    }
}
