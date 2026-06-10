using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketManagementSystem;

namespace TicketSystemTests
{
    [TestFixture]
    internal class TicketManagementTests
    {
        private TicketService _ticketService = new TicketService(new UserRepositoryMock()); 

     
        [Test]
        public void CreateTicket_ValidTicket_ReturnsId()
        {

            var id = _ticketService.CreateTicket(
                "System Crash",
                Priority.Medium,
                "Johan",
                "The system crashed when user performed a search",
                DateTime.UtcNow,
                true); 

            Assert.IsNotNull(id);
            Assert.That(id, Is.TypeOf<int>()); 
        }

        [TestCase("", "Hello world", "Johan")]
        [TestCase("System Crash", "", "Johan")]
        [TestCase("System Crash", "Hello world", "")]
        public void CreateTicket_MissingInputValues_ThrowsInvalidTicketException(string t, string desc, string assignedTo)
        {

            Assert.Throws<InvalidTicketException>(() => _ticketService.CreateTicket(
                t,
                Priority.Medium,
                assignedTo,
                desc,
                DateTime.UtcNow,
                true)); 
        }

        [Test]
        public void CreateTicket_InvalidUser_ThrowsUnknownUserException()
        {

            Assert.Throws<UnknownUserException>(() => _ticketService.CreateTicket(
               "System Crash",
               Priority.Medium,
               "BobbyTables",
               "The system crashed when user performed a search",
               DateTime.UtcNow,
               true)); 
        }
    }
}
