using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Client
    {
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string ClientName { get; set; }
        public string ClientUrl { get; set; }
    }
}
