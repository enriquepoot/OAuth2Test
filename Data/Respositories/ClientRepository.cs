using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Respositories
{
    public class ClientRepository
    {
        private static List<Client> repo;

        public ClientRepository()
        {
            if (repo == null)
            {
                repo = new List<Client>
                {
                    new Client{
                        ClientID = "bXljbGllbnRpZA==",
                        ClientName = "My Application",
                        ClientSecret = "aXQnc2FzZWNyZXQ=",
                        ClientUrl = "myurl"
                    }
                };
            }
        }

        public Client GetClient(string clientID)
        {
            return repo
                .Where(w => w.ClientID == clientID)
                .FirstOrDefault();
        }
    }
}
