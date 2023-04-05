using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFID_WPF_Autorization
{
    public class UserModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string lastname { get; set; }
        public string birthdate { get; set; }

        public int gender { get; set; }
        public string photopath { get; set; }
    }

    public class FullUser
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string lastname { get; set; }
        public string birthdate { get; set; }

        public int gender { get; set; }
        public string photopath { get; set; }
    }


}
