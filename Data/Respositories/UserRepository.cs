using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Respositories
{
    public class UserRepository
    {
        private static List<User> repo;

        public UserRepository()
        {
            repo = new List<User>()
            {
                new User{ UserID = 1, Name = "test", Password= "testing"}
            };
        }

        public User GetUser(int id)
        {
            return repo
                .Where(w => w.UserID == id)
                .FirstOrDefault();
        }
    }
}
