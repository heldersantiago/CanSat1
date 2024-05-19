using CanSat1.models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Xml.Linq;
using CanSat1.services;

namespace CanSat1.services
{
    public class DataService
    {
        private readonly DatabaseService databaseService;
        public DataService(DatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public async Task StoreStatusAsync(string temperature, string humidity, string detected_object, DateTime updated_at)
        {
                using var connection = await databaseService.OpenConnectionAsync();
            
                if (connection == null) return;

                // Insert new status modification in the database
                using (MySqlCommand storeSessionCmd = new MySqlCommand("INSERT INTO states (temperatura,humidade,objecto_detectado, updated_at) VALUES (@Temperature, @Humidity,@DetectedObject,@UpdatedAt)", connection))
                {
                    storeSessionCmd.Parameters.AddWithValue("@Temperature", temperature);
                    storeSessionCmd.Parameters.AddWithValue("@Humidity", humidity);
                    storeSessionCmd.Parameters.AddWithValue("@DetectedObject", detected_object);
                    storeSessionCmd.Parameters.AddWithValue("@UpdatedAt", updated_at);

                    await storeSessionCmd.ExecuteNonQueryAsync();
                }
        }
    }
}