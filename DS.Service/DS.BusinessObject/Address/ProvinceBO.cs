using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject.Address
{
    public class ProvinceBO
    {
        public int PROVINCEID { get; set; }
        public string PROVINCENAME { get; set; }
    }

    public class DistrictBO
    {
        public int PROVINCEID { get; set; }
        public int DISTRICTID { get; set; }
        public string DISTRICTNAME { get; set; }
    }

    public class WardBO
    {
        public int DISTRICTID { get; set; }
        public int WARDID { get; set; }
        public string WARDNAME { get; set; }
    }

}
