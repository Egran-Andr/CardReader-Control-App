using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFID_WPF_Autorization
{
    public class HistoryModel
    {
        public int workerid { get; set; }
        public int workplaceid { get; set; }
        public DateTime entertimestamp { get; set; }
    }

    public class HistoryReturnModel
    {
        public string workerfio { get; set; }
        public string workplacename { get; set; }
        public string entertimestamp { get; set; }
    }

    public class HistoryReturnModelDate
    {
        public string workerfio { get; set; }
        public string workplacename { get; set; }
        public DateTime entertimestamp { get; set; }
    }

    public class HistoryAddModel
    {
        public int workerid { get; set; }
        public int workplaceid { get; set; }
        public string entertimestamp { get; set; }
    }
}
