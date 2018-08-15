using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class AdapterViewModel 
    {
        public AdapterViewModel(ExchangerViewModel exchangerViewModelToBind)
        {
            AdapterManager = new AdapterManager();
            BindAllParameters(exchangerViewModelToBind);
            Parser = new Parser();

            Parser.AnalogParameterArrived += AdapterManager.OnAnalogParameterArrived;
            Parser.DigitalParamterArrived += AdapterManager.OnDigitalParameterArrived;
            Parser.AdapterFound += Parser_AdapterFound;
        }

        private void Parser_AdapterFound(object sender, string name)
        {
            PortName = name;
        }

        public string PortName { get; set; }
        public bool EnableChanges { get; set; }
        public bool IsConnected
        {
            get
            {
                return AdapterManager.IsConnected;
            }
        }
        private AdapterManager AdapterManager;
        private Parser Parser;

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
            AdapterManager.InitializeDigitalInputs(
                (IBindableDigitalInput)exchangerViewModel.SupplyChannel.HVACObjectsList.First(item => item is HVACFan),
                (IBindableDigitalInput)exchangerViewModel.SupplyChannel.HVACObjectsList.First(item => item is HVACCooler),
                (IBindableDigitalInput)exchangerViewModel.SupplyChannel.HVACObjectsList.First(item => item is HVACHeater),
                exchangerViewModel.Exchanger
                );
            AdapterManager.InitializeDigitalOutputs(
                (IBindableDigitalOutput)exchangerViewModel.Exchanger
                );
        }

    }
}
