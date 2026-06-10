using System;
using System.Data.SqlClient;

namespace TicketManagementSystem
{
    public class UserRepository : IUserRepository
    {
        private string connectionString;

        public UserRepository()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["database"].ConnectionString;
        }
        
        public User GetUser(string username)
        {
            // Assume this method does not need to change and is connected to a database with users populated.
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    string sql = "SELECT TOP 1 FROM Users u WHERE u.Username == @p1";
                    connection.Open();

                    var command = new SqlCommand(sql, connection)
                    {
                        CommandType = System.Data.CommandType.Text,
                    };

                    command.Parameters.Add("@p1", System.Data.SqlDbType.NVarChar).Value = username;

                    return (User)command.ExecuteScalar();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public User GetAccountManager()
        {
            // Assume this method does not need to change.
            return GetUser("Sarah");
        }
    }
}
