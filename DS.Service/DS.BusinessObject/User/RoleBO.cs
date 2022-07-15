using System.Collections.Generic;

namespace DS.BusinessObject.User
{
    public class RoleBO
    {
        public int ID { get; set; }
        public string ROLENAME { get; set; }
        public string DESCRIPTION { get; set; }
        public IList<RoleDetailBO> LISTROLEDETAIL { get; set; }
    }
}
