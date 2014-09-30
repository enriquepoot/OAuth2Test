using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class AccessTokenModel
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }

        public AccessTokenModel(AccessToken token)
        {
            this.access_token = token.Token;
            this.token_type = token.Type;
            this.expires = (token.Expiration - DateTime.Now).TotalSeconds.ToString();
            this.refresh_token = token.RefreshToken;
            this.scope = token.Scope;
        }
    }
}
