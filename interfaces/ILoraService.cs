using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanSat1.interfaces
{
    public interface ILoraService : IDisposable
    {
        string Port { get; set; }
        int BaudRate { get; set; }
        bool IsConnected { get; }
        event Action<string> DataReceived;
        bool Connect();
        void Disconnect();
        new void Dispose();
    }
}
