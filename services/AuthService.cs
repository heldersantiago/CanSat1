using CanSat1.Utils;
using CanSat1.Services;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;
using System.Data;
using CanSat1.models;
using CanSat1.interfaces;
using System.Diagnostics;

namespace CanSat1.services
{
    public class AuthService:IAuthService
    {
        private readonly DatabaseService _databaseService;

        public AuthService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                using var connection = await _databaseService.OpenConnectionAsync();
                if (connection == null) return false;

                var query = "SELECT id, name, password FROM users WHERE email = @Email";
                using var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Email", email);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var userId = reader.GetInt32("id");
                    var name = reader.GetString("name");
                    var storedPassword = reader.GetString("password");

                    if (await _Utils.ValidatePasswordAsync(password, storedPassword))
                    {
                        await StoreSessionAsync(userId, name, email);
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                // Log the exception (not shown here)
                return false;
            }
        }

        public async Task<bool> RegisterAsync(User user)
        {
            try
            {
                using var connection = await _databaseService.OpenConnectionAsync();
                if (connection == null)
                {
                    Debug.WriteLine("Erro ao conectar a db");
                    return false;
                }

                var checkUserQuery = "SELECT COUNT(*) FROM users WHERE email = @Email";
                using var checkUserCmd = new MySqlCommand(checkUserQuery, connection);
                checkUserCmd.Parameters.AddWithValue("@Email", user.Email);

                var userCount = (long)await checkUserCmd.ExecuteScalarAsync();
                if (userCount > 0) {
                    Debug.WriteLine(" Email com este user ja existe");
                    return false;
                      }

                var insertUserQuery = "INSERT INTO users (name, email, password) VALUES (@Name, @Email, @Password)";
                using var insertUserCmd = new MySqlCommand(insertUserQuery, connection);
                insertUserCmd.Parameters.AddWithValue("@Name", user.Name);
                insertUserCmd.Parameters.AddWithValue("@Email", user.Email);
                insertUserCmd.Parameters.AddWithValue("@Password", user.Password);

                return await insertUserCmd.ExecuteNonQueryAsync() == 1;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.ToString());
                // Log the exception (not shown here)
                return false;
            }
        }

        private async Task StoreSessionAsync(int userId, string name, string email)
        {
            try
            {
                using var connection = await _databaseService.OpenConnectionAsync();
                if (connection == null) return;

                var insertSessionQuery = "INSERT INTO sessions (user_id, user_name, user_email) VALUES (@UserId, @UserName, @UserEmail)";
                using var insertSessionCmd = new MySqlCommand(insertSessionQuery, connection);
                insertSessionCmd.Parameters.AddWithValue("@UserId", userId);
                insertSessionCmd.Parameters.AddWithValue("@UserName", name);
                insertSessionCmd.Parameters.AddWithValue("@UserEmail", email);

                await insertSessionCmd.ExecuteNonQueryAsync();
            }
            catch
            {
                // Log the exception (not shown here)
            }
        }

        public async Task DestroySessionAsync()
        {
            try
            {
                using var connection = await _databaseService.OpenConnectionAsync();
                if (connection == null) return;

                var truncateSessionQuery = "TRUNCATE sessions";
                using var truncateSessionCmd = new MySqlCommand(truncateSessionQuery, connection);

                await truncateSessionCmd.ExecuteNonQueryAsync();
            }
            catch
            {
                // Log the exception (not shown here)
            }
        }

        public async Task<User?> GetUserFromSessionAsync()
        {
            try
            {
                using var connection = await _databaseService.OpenConnectionAsync();
                if (connection == null) return null;

                var query = "SELECT user_name, user_email FROM sessions";
                using var cmd = new MySqlCommand(query, connection);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                   var name = reader.GetString("user_name");
                   var email = reader.GetString("user_email");

                   var user = new User(name,email,"");

                    return user;
                }

                return null;
            }
            catch
            {
                // Log the exception (not shown here)
                return null;
            }
        }
    }
}
