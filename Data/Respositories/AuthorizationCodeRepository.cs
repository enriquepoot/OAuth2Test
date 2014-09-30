using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Respositories
{
    public class AuthorizationCodeRepository
    {
        private static List<AuthorizationCode> repo;

        public AuthorizationCodeRepository()
        {
            if (repo == null)
                repo = new List<AuthorizationCode>();
        }

        public AuthorizationCode GetAuthorizationCode(string code)
        {
            return repo
                .Where(w => w.Code.Equals(code))
                .FirstOrDefault();
        }

        public bool Save(AuthorizationCode code)
        {
            try
            {
                repo.Add(code);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
