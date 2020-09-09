using System.Collections.Generic;

namespace AspNetCoreDapper.DTO
{
    public class UserGroupDTO
    {
        public UserGroupDTO()
        {
            USERS = new List<UserDTO>();
        }
        public int GROUPiD { get; set; }
        public string NAME { get; set; }
        public IEnumerable<UserDTO> USERS { get; set; }
    }
}
