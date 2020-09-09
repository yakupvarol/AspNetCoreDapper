using AspNetCoreDapper.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreDapper.Models
{
    public class HomeIndexViewModel
    {
        public IList<UserDTO> ListModel { get; set; }
    }
}
