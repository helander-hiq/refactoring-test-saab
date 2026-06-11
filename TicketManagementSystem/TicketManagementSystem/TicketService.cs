using System;
using System.Configuration;
using System.IO;
using System.Text.Json;
using EmailService;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace TicketManagementSystem
{
    public class TicketService
    {
        private IUserRepository UserRepository { get; } 

        public TicketService()
        {
            UserRepository = new UserRepository();
        }

        public TicketService(IUserRepository userRepository)
        {
            UserRepository = userRepository; 
        }

        /// <summary>
        /// Create a ticket
        /// </summary>
        /// <param name="t">Title</param>
        /// <param name="p">Priority</param>
        /// <param name="assignedTo">Username the ticket is assigned to</param>
        /// <param name="desc">Description</param>
        /// <param name="d">Date and time of the ticket</param>
        /// <param name="isPayingCustomer">True if the customer is a paying customer, otherwise false</param>
        /// <returns></returns>
        /// <exception cref="InvalidTicketException">Input values are invalid</exception>
        /// <exception cref="UnknownUserException">The user wasn't found</exception>
        public int CreateTicket(string t, Priority p, string assignedTo, string desc, DateTime d, bool isPayingCustomer)
        {
            (bool isValid, string[] messages) inputValidation  = ValidateCreateTicketInput(t, desc, assignedTo); 
            if (!inputValidation.isValid)
            {
                throw new InvalidTicketException($"Input is not valid. Messages: {string.Join(',', inputValidation.messages)}");
            }

            var user = GetUser(assignedTo);

            if (user == null)
            {
                throw new UnknownUserException($"User {assignedTo} not found");
            }

            var priority = GetTicketPriority(p, d, t);

            if (priority == Priority.High)
            {
                NotifyAdmin(t, assignedTo);
            }

            var price = isPayingCustomer ? GetPrice(p) : 0;

            var accountManager = isPayingCustomer ? GetAccuntManager() : null;

            var ticket = new Ticket()
            {
                Title = t,
                AssignedUser = user,
                Priority = priority,
                Description = desc,
                Created = d,
                PriceDollars = price,
                AccountManager = accountManager
            };

            var id = TicketRepository.CreateTicket(ticket);


            // Return the id
            return id;
        }

        /// <summary>
        /// Assign an existing ticket to a user
        /// </summary>
        /// <param name="id">Id of the ticket</param>
        /// <param name="username">Username to assign the ticket to</param>
        /// <exception cref="UnknownUserException">The user wasn't found</exception>
        /// <exception cref="ApplicationException">The ticket wasn't found</exception>
        public void AssignTicket(int id, string username)
        {
            var user = GetUser(username);

            if (user == null)
            {
                throw new UnknownUserException($"User {username} not found");
            }

            var ticket = TicketRepository.GetTicket(id);

            if (ticket == null)
            {
                throw new ApplicationException("No ticket found for id " + id);
            }

            ticket.AssignedUser = user;

            TicketRepository.UpdateTicket(ticket);
        }

        private void NotifyAdmin(string title, string assignedUser)
        {
            var emailService = new EmailServiceProxy();
            emailService.SendEmailToAdministrator(title, assignedUser);
        }

        private User GetUser(string userName)
        {
            User user = UserRepository.GetUser(userName);
            
            return user; 
        }

        private User GetAccuntManager()
        {
            User manager = UserRepository.GetAccountManager();

            return manager;
        }


        private double GetPrice(Priority priority)
        {
            var price = priority == Priority.High ? 100 : 50;

            return price;
        }

        private (bool isValid, string[] messages) ValidateCreateTicketInput(string title, string description, string assignedTo)
        {
            var messages = new List<string>();
            bool valid = true; 
            if (string.IsNullOrEmpty(title))
            {
                messages.Add("Title is null or empty.");
                valid = false; 
            }
            if (string.IsNullOrEmpty(description))
            {
                messages.Add("Description is null or empty.");
                valid = false; 
            } 

            //This would originally have thrown an unknown user exception and might be considered a functionality change. 
            if(string.IsNullOrEmpty(assignedTo))
            {
                messages.Add("AssignedTo is null or empty.");
                valid = false; 
            }

            return (valid, messages.ToArray()); 
        }

        private Priority GetTicketPriority(Priority currentPriority, DateTime ticketDateTime, string title)
        {
            if (currentPriority == Priority.High)
                return currentPriority;

            //Original code is case sensitive, so I'm keeping this case sensitive to maintain functionality
            var regex = new Regex("(Important|Crash|Failure)+");

            Priority priority = currentPriority;
            
            if((ticketDateTime < (DateTime.UtcNow - TimeSpan.FromHours(1))) ||
                regex.IsMatch(title))
            {
                priority = currentPriority == Priority.Low ? Priority.Medium : Priority.High;
            }

            return priority; 

        }

        private void WriteTicketToFile(Ticket ticket)
        {
            var ticketJson = JsonSerializer.Serialize(ticket);
            File.WriteAllText(Path.Combine(Path.GetTempPath(), $"ticket_{ticket.Id}.json"), ticketJson);
        }
    }

    

    public enum Priority
    {
        High,
        Medium,
        Low
    }
}
