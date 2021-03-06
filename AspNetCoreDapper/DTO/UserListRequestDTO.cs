﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreDapper.DTO
{
    public class UserListRequestDTO
    {
        public int Id { get; set; }
        public int Groupid { get; set; }
        public int Usertypeid { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
