using System;
using System.Collections.Generic;
using System.Text;

namespace DapperTests.DbModels
{
    public class Role
    {
        public Role()
        {
            this.Pages = new List<Page>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public List<Page> Pages { get; set; }
    }
}
