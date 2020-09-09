using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreDapper.DTO
{
    public class QueryGetAllDTO
    {
        public int ID { get; set; }
        public int GROUPID { get; set; }
        public string GROUPNAME { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string PHONE { get; set; }
        
    }
}
