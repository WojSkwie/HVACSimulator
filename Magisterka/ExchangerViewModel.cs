using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HVACSimulator
{
    public class ExchangerViewModel :IDynamicObject
    {
        
        
        public HVACSupplyChannel supplyChannel;
        public HVACExhaustChannel exhaustChannel;
        public List<Image> imagesSupplyChannnel;


        public ExchangerViewModel()
        {
            supplyChannel = new HVACSupplyChannel();
            exhaustChannel = new HVACExhaustChannel();
            imagesSupplyChannnel = new List<Image>();
        }

        public void UpdateParams()
        {
            supplyChannel.UpdateParams();
            exhaustChannel.UpdateParams();
        }

        public double GetSpeedFromSupplyChannel()
        {
            foreach (HVACObject obj in supplyChannel.HVACObjectsList)
            {
                if (obj is HVACFan)
                {
                    return ((HVACFan)obj).ActualSpeedPercent;
                }
            }
            throw new Exception("Brak wentylatora w kanale nawiewnym");
        }

        public double GetHotWaterTempeartureFromSuppyChannel()
        {
            foreach (HVACObject obj in supplyChannel.HVACObjectsList)
            {
                if (obj is HVACHeater)
                {
                    return ((HVACHeater)obj).ActualWaterTemperature;
                }
            }
            throw new Exception("Brak nagrzewnicy w kanale nawiewnym");
        }

        public double GetColdWaterTemperatureFromSupplyChannel()
        {
            foreach (HVACObject obj in supplyChannel.HVACObjectsList)
            {
                if (obj is HVACCooler)
                {
                    return ((HVACCooler)obj).ActualWaterTemperature;
                }
            }
            throw new Exception("Brak chłodnicy w kanale nawiewnym");
        }

        public double GetFlowRateFromSupplyChannel()
        {
            return supplyChannel.FlowRate;
        }

        public double GetPressureDropFromSupplyChannel()
        {
            return supplyChannel.FanPressureDrop;
        }
    }
}
