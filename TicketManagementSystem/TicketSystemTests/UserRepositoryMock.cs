using TicketManagementSystem;

namespace TicketSystemTests
{

    /* 
     * simple alternative to a mocking framework
     */
    public class UserRepositoryMock : IUserRepository
    {

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
