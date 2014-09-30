using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClientApp.Models
{
    public class AccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public string error { get; set; }
    }
}