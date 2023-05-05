using System;
using System.Collections.Generic;
using System.Text;

namespace DapperTests.DbModels
{
    public class User
    {
        public User()
        {
            this.Roles = new List<Role>();
        }

        public int Id { get; set; }

        public string UserName { get; set; }

        public List<Role> Roles { get; set; }
    }
}
