using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketManagementSystem
{
    internal interface IUserRepository : IDisposable
    {
        User GetUser(string username);

        User GetAccountManager(); 

    }
}
