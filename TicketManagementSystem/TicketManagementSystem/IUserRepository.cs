using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketManagementSystem
{
    public interface IUserRepository
    {
        User GetUser(string username);

        User GetAccountManager(); 

    }
}
