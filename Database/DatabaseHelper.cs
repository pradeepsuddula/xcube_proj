using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xcube_proj.Model;

namespace xcube_proj.Database
{
    public class DatabaseHelper
    {
        private readonly string _databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "users.db");

        public DatabaseHelper()
        {
            if (!File.Exists(_databasePath))
            {
                SQLiteConnection.CreateFile(_databasePath);
                CreateUsersTable();
            }
        }

        private void CreateUsersTable()
        {
            Debug.WriteLine("Database Path where usersdb is Saving::" + _databasePath);
            using (var connection = new SQLiteConnection($"Data Source={_databasePath};Version=3;"))
            {
                connection.Open();
                string createTableQuery = @"CREATE TABLE IF NOT EXISTS Users (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Name TEXT NOT NULL,
                                        Age INTEGER NOT NULL,
                                        DateOfBirth TEXT NOT NULL,
                                        ContactNumber TEXT NOT NULL,
                                        ProfilePicturePath TEXT
                                    )";
                SQLiteCommand command = new SQLiteCommand(createTableQuery, connection);
                command.ExecuteNonQuery();
            }
        }

        public void AddUser(string name, int age, DateTime dateOfBirth, string contactNumber, string profilePicturePath)
        {
            Debug.WriteLine("Database Path where usersdb is Saving::" + _databasePath);
            using (var connection = new SQLiteConnection($"Data Source={_databasePath};Version=3;"))
            {
                connection.Open();
                string insertQuery = @"INSERT INTO Users (Name, Age, DateOfBirth, ContactNumber, ProfilePicturePath) 
                                   VALUES (@Name, @Age, @DateOfBirth, @ContactNumber, @ProfilePicturePath)";
                SQLiteCommand command = new SQLiteCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Age", age);
                command.Parameters.AddWithValue("@DateOfBirth", dateOfBirth.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@ContactNumber", contactNumber);
                command.Parameters.AddWithValue("@ProfilePicturePath", profilePicturePath);
                command.ExecuteNonQuery();
            }
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using (var connection = new SQLiteConnection($"Data Source={_databasePath};Version=3;"))
            {
                connection.Open();
                string selectQuery = "SELECT * FROM Users";
                SQLiteCommand command = new SQLiteCommand(selectQuery, connection);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Name = reader["Name"].ToString(),
                            Age = Convert.ToInt32(reader["Age"]),
                            DateOfBirth = DateTime.Parse(reader["DateOfBirth"].ToString()),
                            ContactNumber = reader["ContactNumber"].ToString(),
                            ProfilePicturePath = reader["ProfilePicturePath"].ToString()
                        });
                    }
                }
            }
            return users;
        }
    }
}
