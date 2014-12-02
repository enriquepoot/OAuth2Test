using Data.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Respositories
{
    public class AccessTokenRepository
    {
        private static List<AccessToken> repo;

        public AccessTokenRepository()
        {
            if (repo == null)
                repo = new List<AccessToken>();
        }

        public bool Save(AccessToken token)
        {
            try
            {
                repo.Add(token);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public AccessToken GetAuthToken(string token)
        {
            return repo
                .Where(w => w.Token.Equals(token))
                .FirstOrDefault();
        }

        public AccessToken GetRefreshToken(string refresh_token)
        {
            return repo
                .Where(w => w.RefreshToken.Equals(refresh_token))
                .FirstOrDefault();
        }
    }
}
