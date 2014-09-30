using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class AccessToken
    {
        public int UserID { get; set; }
        public string ClientID { get; set; }
        public string Token { get; set; }
        public string Type { get; set; }
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; }
        public string Scope { get; set; }
    }
}
