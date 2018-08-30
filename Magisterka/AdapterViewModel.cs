using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;

namespace HVACSimulator
{
    public class AdapterViewModel : INotifyPropertyChanged
    {
        public AdapterViewModel(ExchangerViewModel exchangerViewModelToBind)
        {
            Parser = new Parser();
            AdapterManager = new AdapterManager(Parser.ParseCorrectCroppedFrame, OnAdapterStateChange);
            BindAllParameters(exchangerViewModelToBind);

            Parser.AnalogParameterArrived += AdapterManager.OnAnalogParameterArrived;
            Parser.DigitalParamterArrived += AdapterManager.OnDigitalParameterArrived;
            Parser.AdapterFound += Parser_AdapterFound;
            


        }

        private void Parser_AdapterFound(object sender, string name)
        {
            PortName = name;
        }

        private string _PortName;

        public string PortName
        {
            get { return _PortName; }
            set { _PortName = value; OnPropertyChanged("PortName"); }
        }

        public bool AreChangesEnabled { get; set; }
        private bool _IsConnected;

        public bool IsConnected
        {
            get { return _IsConnected; }
            set { _IsConnected = value; OnPropertyChanged("IsConnected"); }
        }

        private void OnAdapterStateChange(object sender, bool newState)
        {
            IsConnected = newState;
        }

        private bool _AllowClick = true;

        public bool AllowClick
        {
            get { return _AllowClick; }
            set { _AllowClick = value; OnPropertyChanged("AllowClick"); }
        }

        private AdapterManager AdapterManager;
        private Parser Parser;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void BindAllParameters(ExchangerViewModel exchangerViewModel)
        {
            AdapterManager.InitializeAnalogInputs(
                (IBindableAnalogInput)exchangerViewModel.SupplyChannel.HVACObjectsList.First(item => item is HVACFan),
                (IBindableAnalogInput)exchangerViewModel.SupplyChannel.HVACObjectsList.First(item => item is HVACCooler),
                (IBindableAnalogInput)exchangerViewModel.SupplyChannel.HVACObjectsList.First(item => item is HVACMixingBox),
                (IBindableAnalogInput)exchangerViewModel.SupplyChannel.HVACObjectsList.First(item => item is HVACHeater)
                );
            AdapterManager.InitializeAnalogOutputs(
                exchangerViewModel.Room,
                exchangerViewModel.Environment,
                exchangerViewModel.SupplyChannel,
                (IBindableAnalogOutput)exchangerViewModel.ExhaustChannel.HVACObjectsList.First(item => item is HVACOutletExchange)
                );
            AdapterManager.InitializeDigitalInputs(
                (IBindableDigitalInput)exchangerViewModel.SupplyChannel.HVACObjectsList.First(item => item is HVACFan),
                (IBindableDigitalInput)exchangerViewModel.SupplyChannel.HVACObjectsList.First(item => item is HVACCooler),
                (IBindableDigitalInput)exchangerViewModel.SupplyChannel.HVACObjectsList.First(item => item is HVACHeater),
                exchangerViewModel.Exchanger
                );
            AdapterManager.InitializeDigitalOutputs(
                exchangerViewModel.Exchanger
                );
        }

        public async Task SearchAdapter()
        {
            await AdapterManager.SendEchoToFindAdapter();
        }

        public bool Connect()
        {
            if (string.IsNullOrEmpty(PortName)) { MessageBox.Show("Wpisz nazwe portu"); return false; }
            PortName = PortName.ToUpper();
            return AdapterManager.Connect(PortName);
        }

        public bool Disconnect()
        {
             return AdapterManager.Disconnect();
        }

        public void SendValuesToAdapter()
        {
            AdapterManager.CreateAndSendDataToAdapter();
        }

        public void SendDataRequestToAdapter()
        {
            AdapterManager.SendRequestForData();
        }
    }
}
