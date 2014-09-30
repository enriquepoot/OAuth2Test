using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Respositories
{
    public class AccessTokenRepository
    {
        private List<AccessToken> repo;

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
    }
}
