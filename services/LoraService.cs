using CanSat1.interfaces;
using System;
using System.IO.Ports;


namespace CanSat1.Services
{
    internal class LoraService : ILoraService
    {
        private  SerialPort serialPort;

        public string Port { get; set; }
        public int BaudRate { get; set; }
        public bool IsConnected => serialPort.IsOpen;
        public event Action<string> DataReceived;

        public LoraService(string port, int baudRate)
        {
            Port = port;
            BaudRate = baudRate;
            serialPort = new SerialPort();
            InitializeSerialPort();
        }

        public LoraService()
        {
            serialPort = new SerialPort();
            InitializeSerialPort();
        }

        private void InitializeSerialPort()
        {
            serialPort.DataReceived += SerialPort_DataReceived;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadExisting();
            DataReceived?.Invoke(data);
        }

        public bool Connect()
        {
            if (IsConnected)
            {
                return true; // Already connected
            }

            serialPort.PortName = Port;
            serialPort.BaudRate = BaudRate;

            try
            {
                serialPort.Open();
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                // Handle unauthorized access 
                return false;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Error connecting to the serial port: {ex.Message}");
                return false;
            }
        }

        public void Disconnect()
        {
            if (IsConnected)
            {
                serialPort.Close();
            }
        }

        public void Dispose()
        {
            Disconnect();
            serialPort.Dispose();
        }

        public void SendMessage(string message)
        {
            if (IsConnected)
            {
                serialPort.Write(message);
            }
        }
    }
}