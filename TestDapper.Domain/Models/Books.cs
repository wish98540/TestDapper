using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Xml.Linq;

namespace TestDapper.Models
{
    public class Books
    {
        public DateTime AddDate { get; set; }
        public string Author { get; set; }
        public string Country { get; set; }
        public int Id { get; set; }
        public string IsRead { get; set; }
        public string Name { get; set; }
        public string ISBN
        {
            get; set;
        }

    }
}