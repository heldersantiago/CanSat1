using CanSat1.models;
using MySql.Data.MySqlClient;

namespace CanSat1.services
{
    public class DataService
    {
        private readonly DatabaseService databaseService;
        public DataService(DatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public async Task StoreStatusAsync(string temperature, string humidity, string gas, DateTime updated_at)
        {
                using var connection = await databaseService.OpenConnectionAsync();
            
                if (connection == null) return;

                // Insert new status modification in the database
                using (MySqlCommand storeSessionCmd = new MySqlCommand("INSERT INTO states (temperatura,humidade,gas, updated_at) VALUES (@Temperature, @Humidity,@Gas,@UpdatedAt)", connection))
                {
                    storeSessionCmd.Parameters.AddWithValue("@Temperature", temperature);
                    storeSessionCmd.Parameters.AddWithValue("@Humidity", humidity);
                    storeSessionCmd.Parameters.AddWithValue("@Gas", gas);
                    storeSessionCmd.Parameters.AddWithValue("@UpdatedAt", updated_at);

                    await storeSessionCmd.ExecuteNonQueryAsync();
                }
        }

        public async Task<List<Sensor>> GetStatesAsync()
        {
            List<Sensor> states = new List<Sensor>();

            using (MySqlConnection connection = await databaseService.OpenConnectionAsync())
            {
                if (connection == null)
                {
                    // Failed to open a database connection
                    return null; // or handle the error appropriately
                }

                using MySqlCommand cmd = new MySqlCommand("SELECT * FROM states", connection);

                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader == null) return null;

                while (await reader.ReadAsync())
                {
                    // Read user data from the reader
                    string temperatura = reader.GetString("temperatura") ?? null;
                    string humidade = reader.GetString("humidade") ?? null;
                    string gas = reader.GetString("gas");
                    DateTime updated_at = reader.GetDateTime("updated_at");

                    Sensor _state = new Sensor { Temperature = temperatura, Humidity = humidade, Gas = gas, UpdatedAt = updated_at };
                    states.Add(_state);
                }
            }
            return states;
        }
    }
}