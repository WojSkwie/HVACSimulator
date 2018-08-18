using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class AdapterViewModel : INotifyPropertyChanged
    {
        public AdapterViewModel(ExchangerViewModel exchangerViewModelToBind)
        {
            Parser = new Parser();
            AdapterManager = new AdapterManager(Parser.ParseCorrectCroppedFrame);
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
        public bool IsConnected
        {
            get
            {
                return AdapterManager.IsConnected;
            }
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
                (IBindableAnalogOutput)exchangerViewModel.Room
                );
            /*AdapterManager.InitializeDigitalInputs(
                (IBindableDigitalInput)exchangerViewModel.SupplyChannel.HVACObjectsList.First(item => item is HVACFan),
                (IBindableDigitalInput)exchangerViewModel.SupplyChannel.HVACObjectsList.First(item => item is HVACCooler),
                (IBindableDigitalInput)exchangerViewModel.SupplyChannel.HVACObjectsList.First(item => item is HVACHeater),
                exchangerViewModel.Exchanger
                );
            AdapterManager.InitializeDigitalOutputs(
                (IBindableDigitalOutput)exchangerViewModel.Exchanger
                );*/
        }

        public void SearchAdapter()
        {
            AdapterManager.SendEchoToFindAdapter();
        }

    }
}
