using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFID_WPF_Autorization
{
    public class WorkplaceModel
    {
        public string Name { get; set; }
    }
    public class WorkplaceReturnModel
    {
        public int id { get; set; }
        public string Name { get; set; }
    }

    public class WorkplaceUserConnection
    {
        public int workerid { get; set; }
        public int workplaceid { get; set; }
    }
}
