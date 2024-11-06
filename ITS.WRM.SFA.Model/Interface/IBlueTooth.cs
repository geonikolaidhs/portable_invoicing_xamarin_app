using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Model.Interface
{
    public interface IBlueTooth
    {
        Task Start(string name, eBlueToothDevice type);
        void Cancel();
        ObservableCollection<string> PairedDevices();

        bool Connected();
        Task PrintTestPage(string deviceName);
        Task PrintDocument(string deviceName, DocumentHeader docHeader);
    }
}
