using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreDapper.DTO
{
    public class GroupOneToOneDTO
    {
        public int GROUPiD { get; set; }
        public string NAME { get; set; }
        public UserOneDTO USERS { get; set; }
    }

    public class UserOneDTO
    {
        public int USERiD { get; set; }
        public int USERTYPEiD { get; set; }
        public int GROUPiD { get; set; }
        public string EMAiL { get; set; }
        public string FiRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string PASSWORD { get; set; }
        public string PHONE { get; set; }
        public GroupOneToOneDTO USER_GROUPS { get; set; }
    }

    public class QueryMulitpleOther
    {
        public IEnumerable<UserOneDTO> USERS { get; set; }
        public IEnumerable<GroupOneToOneDTO> USER_GROUPS { get; set; }
    }
}
