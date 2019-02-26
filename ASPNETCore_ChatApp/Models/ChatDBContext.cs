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
        public string ConnectionString { get; set; }

        public ChatDBContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        private string CalculateTimeSinceInserted(string timeStamp)
        {
            DateTime dateThen = Convert.ToDateTime(timeStamp);
            DateTime dateNow = DateTime.Now;
            int numOfMilliseconds = (int)dateNow.Subtract(dateThen).TotalMilliseconds;
            int seconds = numOfMilliseconds / 1000;
            int minutes = seconds / 60;
            int hours = minutes / 60;
            int days = hours / 24;
            int months = days / 30;
            int years = months / 12;

            if (seconds < 60)
            {
                if (seconds == 1)
                    return seconds + " second ago";
                else
                    return seconds + " seconds ago";
            }
            else if (minutes < 60)
            {
                if (minutes == 1)
                    return minutes + " minute ago";
                else
                    return minutes + " minutes ago";
            }
            else if (hours < 24)
            {
                if (hours == 1)
                    return hours + " hour ago";
                else
                    return hours + " hours ago";
            }
            else if (days < 30)
            {
                if (days == 1)
                    return days + " day ago";
                else
                    return days + " days ago";
            }
            else if (months >= 60 || months <= 365)
            {
                if (months == 1)
                    return months + " month ago";
                else
                    return months + " months ago";
            }
            else
            {
                if (years == 1)
                    return years + " year ago";
                else
                    return years + " years ago";
            }
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

        public User GetUserByUname(string uname)
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
                        user = new User()
                        {
                            Id = Convert.ToInt32(reader["user_id"]),
                            Username = reader["username"].ToString(),
                            Email = reader["email"].ToString(),
                            Password = reader["password"].ToString(),
                            Status = reader["status"].ToString(),
                            DateJoined = CalculateTimeSinceInserted(reader["date_joined"].ToString()),
                            Image = reader["image"].ToString()
                        };
                    }
                    return user;
                }
            }

        }

        public bool GetOnlineUser(string uname)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM is_online " +
                    "WHERE username = ?username";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("?username", uname);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        return true;
                    }
                    return false;
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
                        onlineUsers.Add(new{
                            Id = Convert.ToInt32(reader["user_id"]),
                            Username = reader["username"].ToString(),
                            Email = reader["email"].ToString(),
                            Password = reader["password"].ToString(),
                            Image = reader["image"].ToString(),
                            Status = reader["status"].ToString(),
                            Date = CalculateTimeSinceInserted(reader["date"].ToString()),
                            ConnectionId = reader["connection_id"].ToString()
                        });
                    }
                    return onlineUsers;
                }
            }
        }

        public void UpdateUserStatus(string username, string status)
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string sql = "UPDATE user " +
                        "SET status = ?status " +
                        "WHERE username = ?username";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("?username", username);
                    cmd.Parameters.AddWithValue("?status", status);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("mysql error: " + ex.Message);
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



        public dynamic SetOnlineUser(string username, string connection_id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("GetUserByUname(username) is null: " + GetOnlineUser(username) is null);

                if (GetOnlineUser(username)) //if user is online
                {
                    UpdateOnlineUser(username, connection_id);
                }
                else // if user is not online
                {
                    InsertOnlineUser(username, connection_id);
                }

                return GetOnlineUsers();
            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("mysql error: " + ex.Message);
                return "mysql error: " + ex.Message;
            }
        }

        public void UpdateOnlineUser(string username, string connection_id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("GetUserByUname(username) is null: " + GetOnlineUser(username) is null);

                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    string sql = "UPDATE is_online " +
                            "SET connection_id = ?connection_id " +
                            "WHERE username = ?username";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("?connection_id", connection_id);
                    cmd.Parameters.AddWithValue("?username", username);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("mysql error: " + ex.Message);
            }
        }

        public void InsertOnlineUser(string username, string connection_id)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("GetUserByUname(username) is null: " + GetOnlineUser(username) is null);

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

            }
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("mysql error: " + ex.Message);
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
