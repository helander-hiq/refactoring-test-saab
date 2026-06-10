using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketManagementSystem
{

    /* 
     * This class is just for testing. 
     * The  limitations complicates using DI so I created this instead of setting up a database.
     */
    internal class UserRepositoryMock : IUserRepository
    {
        public void Dispose()
        {
            //nothing to dispose
        }

        public User GetAccountManager()
        {
            return new User
            {
                FirstName = "Sarah",
                LastName = "Suggs",
                Username = "Sarah"
            }; 
        }

        public User GetUser(string username)
        {
            if (username == "Sarah")
            {
                return GetAccountManager();
            }
            else if (username == "Johan")
            {
                return new User
                {
                    FirstName = "Johan",
                    LastName = "Tomson",
                    Username = "Johan"
                };
            }
            else if (username == "Michael")
            {
                return new User
                {
                    FirstName = "Michael",
                    LastName = "Godswallow",
                    Username = "Michael"
                };
            }

            return null; 
        }
    }
}
