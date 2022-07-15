using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.BusinessObject
{
    public class SystemErrorBO
    {
        public string Title { get; set; }
        public string Link { get; set; }

        public enum ErrorTypes
        {
            No_Error = 0,
            LoadInfo = 1,
            Insert = 2,
            Update = 3,
            Delete = 4,
            SearchData = 5,
            GetData = 6,
            InvalidIV = 7,
            TokenNotExist = 8,
            TokenInvalid = 9,
            CheckData = 10,
            Others = 11,
            UserDefine = 12
        }
    }
}
