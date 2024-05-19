using CanSat1.interfaces;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace CanSat1.services
{
    public class DatabaseService:IDatabaseService 
    {
        private readonly string _connectionString;
        public DatabaseService(string connectionString = $"Server=localhost;Database=lora_project;User ID=root;Password=heldersantiago273@;")
        {
            // Adjust the connection string based on your MySQL server configuration
            _connectionString = connectionString;
        }

        public async Task<MySqlConnection> OpenConnectionAsync()
        {
            try
            {
                var connection = new MySqlConnection(_connectionString);    
                await connection.OpenAsync();
                return connection;
            }
            catch (Exception ex)
            {
                // Handle connection opening exceptions
                Debug.WriteLine($"Error opening database connection: {ex.Message}");
                return null;
            }
        }

        public static async Task CloseConnectionAsync(MySqlConnection connection)
        {
            try
            {
                await connection.CloseAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error closing database connection: {ex.Message}");
            }
        }
    }
}