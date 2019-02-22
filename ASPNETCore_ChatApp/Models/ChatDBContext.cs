using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCore_ChatApp.Models
{
    public class ChatDBContext
    {
        IHttpContextAccessor contextAccessor;
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
                string sql = "SELECT * FROM user " +
                    "WHERE username = ?username AND password = ?password";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("?username", uname);
                cmd.Parameters.AddWithValue("?password", pwrd);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User(contextAccessor)
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

        public dynamic GetUserByUname(string uname)
        {
            User user = null;

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM user " +
                    "WHERE username = ?username";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("?username", uname);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User(contextAccessor)
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

        public dynamic GetOnlineId(string username)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("getOnlineId", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new MySqlParameter("username", MySqlDbType.VarChar));
                    cmd.Parameters["username"].Value = username;
                    cmd.Parameters["username"].Direction = System.Data.ParameterDirection.Input;
                    cmd.Parameters.Add(new MySqlParameter("online_id", MySqlDbType.Int32));
                    cmd.Parameters["online_id"].Direction = System.Data.ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    return (int)cmd.Parameters["online_id"].Value;
                }
            }
            catch (Exception ex)
            {
               System.Diagnostics.Debug.WriteLine(ex.ToString());
                return ex.Message;
            }

        }

        public dynamic GetOnlineUsers()
        {
            List<object> onlineUsers = new List<object>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM is_online o " +
                    "INNER JOIN user u ON o.username = u.username ";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        onlineUsers.Add(new
                        {
                            Id = Convert.ToInt32(reader["user_id"]),
                            Username = reader["username"].ToString(),
                            Email = reader["email"].ToString(),
                            Password = reader["password"].ToString(),
                            Image = reader["image"].ToString(),
                            ConnectionId = reader["connection_id"].ToString()
                        });
                    }
                    return onlineUsers;
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
                    string sql = "INSERT INTO user(username, email, password, image) " +
                        "VALUES(?uname, ?email, ?pwrd, ?image)";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("?uname", user.Username);
                    cmd.Parameters.AddWithValue("?email", user.Email);
                    cmd.Parameters.AddWithValue("?pwrd", user.Password);
                    cmd.Parameters.AddWithValue("?image", user.Image ?? "user.png");
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

        public dynamic InsertOnlineUser(string username, string connection_id)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string sql = "INSERT INTO is_online(connection_id, username) " +
                        "VALUES(?connection_id, ?username)";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("?connection_id", connection_id);
                    cmd.Parameters.AddWithValue("?username", username);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                return GetOnlineUsers();
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("mysql error: " + ex.Message);
                return "mysql error: " + ex.Message;
            }
        }

        public dynamic RemoveOnlineUser(string username)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string sql = "DELETE FROM is_online " +
                        "WHERE online_id = ?id";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    //cmd.Parameters.AddWithValue("?connection_id", connection_id);
                    cmd.Parameters.AddWithValue("?id", GetOnlineId(username));
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                return GetOnlineUsers();
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("mysql error: " + ex.Message);
                return "mysql error: " + ex.Message;
            }
        }
    }
}
