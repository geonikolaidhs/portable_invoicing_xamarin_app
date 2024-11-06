using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ITS.WRM.SFA.Droid.Classes.Helpers;
using ITS.WRM.SFA.Model.Enumerations;
using ITS.WRM.SFA.Model.Interface;
using ITS.WRM.SFA.Model.Model;
using Java.IO;
using Java.Util;

[assembly: Xamarin.Forms.Dependency(typeof(ITS.WRM.SFA.Droid.Classes.Devices.BlueToothAdapter))]
namespace ITS.WRM.SFA.Droid.Classes.Devices
{
    public class BlueToothAdapter : IBlueTooth
    {
        private CancellationTokenSource _CancelationToken { get; set; }

        private static bool _Connected { get; set; } = false;

        private static bool _Connecting = false;

        private static object _Lock = new object();

        private static string _Status { get; set; }

        private static string _ConnectedDeviceName { get; set; }

        private static string _LastMessage { get; set; }

        private static int _SleepTime = 200;

        private static bool _ReadAsCharArray = false;

        private static BluetoothSocket BthSocket { get; set; }

        public BlueToothAdapter()
        {
        }


        /// <summary>
        /// Start the "reading" loop 
        /// </summary>
        /// <param name="name">Name of the paired bluetooth device </param>
        public async Task Start(string name, eBlueToothDevice type)
        {
            try
            {
                if (!_Connecting)
                {
                    await Task.Run(() => Reading(name, type)).ConfigureAwait(false);
                    _Connecting = false;
                }
            }
            catch (System.Exception ex)
            {
                App.LogError(ex);
            }
            finally
            {
                _Connecting = false;
            }
        }

        /// <summary>
        /// Cancel the Reading loop
        /// </summary>
        /// <returns><c>true</c> if this instance cancel ; otherwise, <c>false</c>.</returns>
        public void Cancel()
        {
            if (_CancelationToken != null)
            {
                _CancelationToken.Cancel();
                _Connected = false;
            }
        }


        public ObservableCollection<string> PairedDevices()
        {
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            ObservableCollection<string> devices = new ObservableCollection<string>();
            foreach (var bd in adapter.BondedDevices)
            {
                UUID uuid = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
                var id = bd.GetUuids().ElementAt(0);
                if (id.ToString().ToUpper() == uuid.ToString().ToUpper())
                {
                    devices.Add(bd.Name);
                }
            }
            return devices;
        }

        public bool Connected()
        {
            return _Connected;
        }


        private async Task Reading(string name, eBlueToothDevice type)
        {
            _Connecting = true;
            _CancelationToken = new CancellationTokenSource();
            BluetoothDevice device = null;
            InputStreamReader mReader = null;
            BufferedReader buffer = null;
            BthSocket = null;

            while (_CancelationToken.IsCancellationRequested == false)
            {
                BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
                if (adapter == null)
                    return;
                if (!adapter.IsEnabled)
                    throw new Exception("Bluetooth Adapter is disabled");

                if (!_Connected && adapter != null)
                {
                    try
                    {
                        lock (_Lock)
                        {
                            foreach (var bd in adapter.BondedDevices)
                            {
                                if (bd.Name.ToUpper().IndexOf(name.ToUpper()) >= 0)
                                {
                                    device = adapter.GetRemoteDevice(bd.Address);
                                    break;
                                }
                            }

                            if (device != null && name == device.Name)
                            {
                                var id = device.GetUuids().ElementAt(0);
                                UUID uuid = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
                                if (id.ToString().ToUpper() == uuid.ToString().ToUpper())
                                {
                                    try
                                    {
                                        if (BthSocket == null)
                                            BthSocket = device.CreateInsecureRfcommSocketToServiceRecord(uuid);

                                        adapter.CancelDiscovery();
                                        bool isDiscovering = adapter.IsDiscovering;
                                        if (isDiscovering)
                                            System.Threading.Thread.Sleep(5000);
                                    }
                                    catch (Java.IO.IOException e)
                                    {
                                        App.LogError(e);
                                    }
                                }
                                if (BthSocket != null && _Connected == false)
                                {
                                    try
                                    {
                                        BthSocket.Connect();
                                    }
                                    catch (Java.IO.IOException ex)
                                    {
                                        //App.LogError(ex);
                                    }
                                }
                            }
                        }

                        if (BthSocket != null && BthSocket.IsConnected && _Connected == false)
                        {
                            _Connected = true;
                            if (type == eBlueToothDevice.SCANNER)
                            {
                                mReader = new InputStreamReader(BthSocket.InputStream);
                                buffer = new BufferedReader(mReader);
                                while (BthSocket != null && BthSocket.IsConnected)
                                {
                                    if (buffer.Ready())
                                    {
                                        char[] chr = new char[128];
                                        string barcode = "";
                                        if (_ReadAsCharArray)
                                        {
                                            await buffer.ReadAsync(chr);
                                            foreach (char c in chr)
                                            {
                                                if (c == '\0')
                                                    break;
                                                barcode += c;
                                            }
                                        }
                                        else
                                            barcode = await buffer.ReadLineAsync();

                                        if (barcode.Length > 0)
                                        {
                                            Xamarin.Forms.MessagingCenter.Send<App, string>((App)Xamarin.Forms.Application.Current, EventNames.SCAN_EVENT, barcode);
                                        }
                                    }
                                    System.Threading.Thread.Sleep(_SleepTime);

                                    if (!BthSocket.IsConnected)
                                    {
                                        _Connected = false;
                                        throw new System.Exception("BthSocket Is Not Connected");
                                    }
                                    if (_CancelationToken.IsCancellationRequested)
                                    {
                                        throw new System.Exception("CancelationToken Requested ");
                                    }
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        //App.LogError(ex);
                        if (BthSocket != null)
                            BthSocket.Close();
                        if (buffer != null)
                            buffer.Close();
                        if (mReader != null)
                            mReader.Close();
                        mReader = null;
                        buffer = null;
                        BthSocket = null;
                        device = null;
                        adapter = null;
                        _Connected = false;
                        _Connecting = false;
                    }
                }
                System.Threading.Thread.Sleep(1000);
            }
            _Connecting = false;
        }


        public async Task PrintTestPage(string deviceName)
        {
            bool zpl = App.SFASettings.Zpl;
            if (_Connected)
            {
                try
                {
                    List<byte> outputList = new List<byte>();
                    string text = "TEST test";
                    byte[] bytes;
                    if (App.SFASettings.PrinterConvertEncoding && App.SFASettings.EncodingTo != null && App.SFASettings.EncodingFrom != null)
                    {
                        bytes = Encoding.Convert(App.SFASettings.EncodingFrom, App.SFASettings.EncodingTo, App.SFASettings.EncodingFrom.GetBytes(text));
                    }
                    else
                    {
                        bytes = Encoding.UTF8.GetBytes(text);
                    }
                    outputList.AddRange(bytes);
                    outputList.Add(0x0A);
                    outputList.Add(0x0A);
                    outputList.Add(0x0A);
                    outputList.Add(AsciiControlChars.EndOfTransmission);
                    await BthSocket.OutputStream.WriteAsync(outputList.ToArray(), 0, outputList.Count());


                    outputList = new List<byte>(); outputList = new List<byte>();
                    text = "qwertyuiopasdfghjlzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";
                    if (App.SFASettings.PrinterConvertEncoding)
                    {
                        bytes = Encoding.Convert(App.SFASettings.EncodingFrom, App.SFASettings.EncodingTo, App.SFASettings.EncodingFrom.GetBytes(text));
                    }
                    else
                    {
                        bytes = Encoding.UTF8.GetBytes(text);
                    }
                    outputList.AddRange(bytes);
                    outputList.Add(0x0A);
                    outputList.Add(0x0A);
                    outputList.Add(0x0A);
                    outputList.Add(AsciiControlChars.EndOfTransmission);
                    await BthSocket.OutputStream.WriteAsync(outputList.ToArray(), 0, outputList.Count());


                    outputList = new List<byte>();
                    text = "ςερτυθιοπασδφγηξκλζχψωβνμςΕΡΤΥΘΙΟΠΑΣΔΦΓΗΞΚΛΖΧΨΩΒΝΜ";
                    if (App.SFASettings.PrinterConvertEncoding)
                    {
                        bytes = Encoding.Convert(App.SFASettings.EncodingFrom, App.SFASettings.EncodingTo, App.SFASettings.EncodingFrom.GetBytes(text));
                    }
                    else
                    {
                        bytes = Encoding.UTF8.GetBytes(text);
                    }
                    outputList.AddRange(bytes);
                    outputList.Add(0x0A);
                    outputList.Add(0x0A);
                    outputList.Add(0x0A);
                    outputList.Add(AsciiControlChars.EndOfTransmission);
                    await BthSocket.OutputStream.WriteAsync(outputList.ToArray(), 0, outputList.Count());

                    outputList = new List<byte>();
                    text = "1,3,5,7,9,11,13,15,17,19,21,23,25,27,29,31,33,35,37,39,41,43,45,47,49,51,53,55,57,59,61,63,65,67,69,71,73,75,79";
                    if (App.SFASettings.PrinterConvertEncoding)
                    {
                        bytes = Encoding.Convert(App.SFASettings.EncodingFrom, App.SFASettings.EncodingTo, App.SFASettings.EncodingFrom.GetBytes(text));
                    }
                    else
                    {
                        bytes = Encoding.UTF8.GetBytes(text);
                    }
                    outputList.AddRange(bytes);
                    outputList.Add(0x0A);
                    outputList.Add(0x0A);
                    outputList.Add(0x0A);
                    outputList.Add(AsciiControlChars.EndOfTransmission);
                    await BthSocket.OutputStream.WriteAsync(outputList.ToArray(), 0, outputList.Count());
                }
                catch (Exception ex)
                {
                    App.LogError(ex);
                    throw ex;
                }
            }
        }

        public async Task PrintDocument(string deviceName, DocumentHeader docHeader)
        {
            if (_Connected)
            {
                try
                {
                    List<byte> bw = new List<byte>();
                    PrintDocumentHelper.PrintDocument(bw, docHeader);
                    await BthSocket.OutputStream.WriteAsync(bw.ToArray(), 0, bw.Count());
                    await Task.Delay(5000);
                    await BthSocket.OutputStream.WriteAsync(bw.ToArray(), 0, bw.Count());
                    BthSocket.OutputStream.Flush();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new Exception("Printer is not Connected");
            }
        }

    }
}

