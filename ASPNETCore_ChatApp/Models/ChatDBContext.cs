using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCore_ChatApp.Models
{
    public class ChatDBContext
    {
        public string ConnectionString { get; set; }

        public ChatDBContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public dynamic GetUser(string uname, string pwrd)
        {
            User user = null;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM user";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User()
                        {
                            Id = Convert.ToInt32(reader["user_id"]),
                            Username = reader["username"].ToString(),
                            Email = reader["email"].ToString(),
                            Password = reader["password"].ToString(),
                            Image = reader["image"].ToString()
                        };
                    }
                    return user;
                }
            }
            
        }

        public dynamic InsertUser(User user)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string sql = "INSERT INTO user(username, email, password, image)" +
                        "VALUES(?uname, ?email, ?pwrd, ?image)";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("?uname", user.Username);
                    cmd.Parameters.AddWithValue("?email", user.Email);
                    cmd.Parameters.AddWithValue("?pwrd", user.Password);
                    if (user.Image != null) cmd.Parameters.AddWithValue("?image", user.Image);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                return GetUser(user.Username, user.Password);
            }catch(MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("mysql error: "+ex.Message);
                return "mysql error: " + ex.Message;
            }
        }
    }
}
