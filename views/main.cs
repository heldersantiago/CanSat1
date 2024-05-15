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

namespace CanSat1.views
{
    public partial class main : Form
    {
        private LoraService loraService;
        private BlynkService blynkService;
        private AuthService authService;
        private User currentUser;

        private readonly string token = "Z7eW3eBCG1ayF8LkF1lozuPX2W-OQr_-";

        public main()
        {
            InitializeComponent();
            initializeServices();


            // On App Initialization load all available Serial Ports on PC, just on APP Initialization
            string[] ports = SerialPort.GetPortNames();
            cbPort.Items.AddRange(ports);
        }

        private void main_Load(object sender, EventArgs e)
        {

        }

        private void main_FormClosed(object sender, FormClosedEventArgs e)
        {
            loraService.Dispose();
        }

        private async void initializeServices()
        {
            loraService = new LoraService();
            blynkService = new BlynkService(token);
            var databaseService = new DatabaseService();
            authService = new(databaseService);

            currentUser = await authService.GetUserFromSessionAsync();
            loraService.DataReceived += LoraService_DataReceived;

            lbUserLoggegIn.Text = $"Olá, {currentUser.Name}";

        }

        private async void LoraService_DataReceived(string dataReceived)
        {
            if (IsValidJson(dataReceived))
            {

                var sensors = JsonConvert.DeserializeObject<Sensor>(dataReceived);
                System.Console.WriteLine(sensors);

                _ = this.Invoke((MethodInvoker)async delegate
                {
                    _Utils.updateSensorData(circleTemperature, sensors.Temperature);
                    _Utils.updateSensorData(circleHumidity, sensors.Humidity, false);


                    // actualiza os dados no bkynk usando o servico "blynkservice"
                    await blynkService.UpdateVirtualPinAsync("v1", float.Parse(sensors.Temperature));
                    await blynkService.UpdateVirtualPinAsync("v2", float.Parse(sensors.Humidity));
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
    }
}
