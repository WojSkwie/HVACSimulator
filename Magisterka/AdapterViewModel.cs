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

        }
        private AdapterManager AdapterManager = new AdapterManager();

        private USB USB = new USB();
        private Parser Parser = new Parser();

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
