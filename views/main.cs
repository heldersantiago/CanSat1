using CanSat1.models;
using CanSat1.Services;
using CanSat1.Utils;
using Guna.UI2.WinForms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NexusUtils.BlynkIntegration;
using CanSat1.services;
using CanSat1.views.auth;
using System.Timers;
using Timer = System.Timers.Timer;
using System.Diagnostics;
using CanSat1.services;


namespace CanSat1.views
{
    public partial class main : Form
    {
        private LoraService loraService;
        private BlynkService blynkService;
        private AuthService authService;
        private DataService dataService;
        private User currentUser;
        private Timer storeDataTimer;
        private List<Sensor> sensorList;

        private readonly string token = "HRbOKqSm3nAoVR9iox2Pn3D8utIALof4";

        public main()
        {
            InitializeComponent();
            initializeServices();

            // On App Initialization load all available Serial Ports on PC, just on APP Initialization
            string[] ports = SerialPort.GetPortNames();
            cbPort.Items.AddRange(ports);


            sensorList = new List<Sensor>();
            InitializeStoreDataTimer();
        }

        private void InitializeStoreDataTimer()
        {
            storeDataTimer = new Timer();
            storeDataTimer.Interval = 1000;
            storeDataTimer.Elapsed += OnStoreDataTimerElapser;
            storeDataTimer.AutoReset = true;
            storeDataTimer.Enabled = true;
        }

        private async Task UpdateDataGridViewAsync()
        {
            // Get all users from the database and update the DataGridView
            dataGridStates.DataSource = await dataService.GetStatesAsync();
        }

        private void OnStoreDataTimerElapser(object? sender, ElapsedEventArgs e)
        {
            if (sensorList.Any())
            {
                StoreSensorData(sensorList);
                sensorList.Clear();
            }
        }

        private async void StoreSensorData(List<Sensor> sensorList)
        {
            if (IsValidJson(sensorList.ToString()))
            {
                Debug.WriteLine(sensorList);
                foreach (var sensor in sensorList)
                {
                    // store the sensor values
                    await dataService.StoreStatusAsync(sensor.Temperature, sensor.Humidity, sensor.Obstacle, DateTime.Now);
                    //Debug.WriteLine($" temp: {sensor.Temperature}, hum {sensor.Humidity}, obj:{sensor.Obstacle} ");
                }
            }
        }

        private void main_Load(object sender, EventArgs e)
        {

        }

        private void main_FormClosed(object sender, FormClosedEventArgs e)
        {
            loraService.Dispose();
            storeDataTimer.Dispose();
        }

        private async void initializeServices()
        {
            loraService = new LoraService();
            blynkService = new BlynkService(token);
            var databaseService = new DatabaseService();
            authService = new(databaseService);
            dataService = new(databaseService);

            currentUser = await authService.GetUserFromSessionAsync();
            loraService.DataReceived += LoraService_DataReceived;

            lbUserLoggegIn.Text = $"Olá, {currentUser.Name}";
            lbName.Text = currentUser.Name;
            lbEmail.Text = currentUser.Email;

            await UpdateDataGridViewAsync();
        }

        private void LoraService_DataReceived(string dataReceived)
        {
            Debug.WriteLine(dataReceived);
            if (IsValidJson(dataReceived))
            {
                var sensors = JsonConvert.DeserializeObject<Sensor>(dataReceived);
                sensorList.Add(sensors);

                _ = this.Invoke((MethodInvoker)async delegate
                {
                    _Utils.updateSensorData(circleTemperature, sensors.Temperature);
                    _Utils.updateSensorData(circleHumidity, sensors.Humidity, false);
                    _Utils.UpdateSinalizerPicture(gbSinalizador, sensors.Obstacle == "true" ? true : false);



                    // actualiza os dados no bkynk usando o servico "blynkservice"
                    await blynkService.UpdateVirtualPinAsync("v1", float.Parse(sensors.Temperature));
                    await blynkService.UpdateVirtualPinAsync("v2", float.Parse(sensors.Humidity));
                    await blynkService.UpdateVirtualPinAsync("v0", sensors.Obstacle == "true" ? 1025 : 0);
                });
            }
        }



        // Valida o dado Json recebido; Ex; {"temperature":12,"humidity":72} e valido, {"temperature":12,"humidity",7} e invalido
        private bool IsValidJson(string str)
        {
            try
            {
                JToken.Parse(str);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }


        private void btnConnection_Click(object sender, EventArgs e)
        {
            if (!loraService.IsConnected)
            {
                if (string.IsNullOrEmpty(cbPort.Text)) { MessageDialog.Show("Selecione uma porta", MessageDialogStyle.Dark); return; }
                if (string.IsNullOrEmpty(cbBaude.Text)) { MessageDialog.Show("Selecione um Baude Rate", MessageDialogStyle.Dark); return; }



                loraService.Port = cbPort.Text;
                loraService.BaudRate = int.Parse(cbBaude.Text);

                if (loraService.Connect())
                {
                    _Utils.UpdateUIOnConnection(btnConnection, true, tabMain);
                }
                else
                {
                    _Utils.ShowConnectionErrorMessage("Falha ao estabelecer conexão");
                }
            }
            else
            {
                loraService.Disconnect(); // If is not connected keep sure the disconnection
                _Utils.UpdateUIOnConnection(btnConnection, false, tabMain);
            }
        }

        private async void btnLogout_Click(object sender, EventArgs e)
        {
            _Utils.HandleBtnOnLoading(btnLogout, "Terminado a sessão...", false);
            if (loraService.IsConnected)
            {
                _Utils.HandleBtnOnLoading(btnLogout, "Terminar sessão", true);
                _Utils.ShowConnectionErrorMessage("Desconecta-se primeiro!");
                return;
            }
            await authService.DestroySessionAsync();
            Login _login = new();
            this.Hide();
            _login.Show();
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
