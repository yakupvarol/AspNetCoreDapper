using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreDapper.DTO
{
    public class UserDTO
    {
        public int UseriD { get; set; }
        public int USERTYPEiD { get; set; }
        public int GROUPiD { get; set; }
        public string EMAiL { get; set; }
        public string FiRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string PASSWORD { get; set; }
        public string PHONE { get; set; }
        public UserGroupDTO USER_GROUPS { get; set; }
    }
}
