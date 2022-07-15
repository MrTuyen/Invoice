using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.User
{
    public class RoleDetailBO
    {
        public int ID { get; set; }
        public int ROLEID { get; set; }
        public string ROLEDETAILNAME { get; set; }
        public string DESCRIPTION { get; set; }
        public bool ISSELECTED { get; set; }
    }
}
