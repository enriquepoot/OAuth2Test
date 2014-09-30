using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class AuthorizationCode
    {
        public string ClientID { get; set; }
        public int UserID { get; set; }
        public string Code { get; set; }
        public string Redirect_Uri { get; set; }
        public DateTime Expiration { get; set; }
    }
}
