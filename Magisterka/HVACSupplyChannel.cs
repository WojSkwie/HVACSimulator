using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Magisterka
{
    public sealed class HVACSupplyChannel : AirChannel, IDynamicObject, INotifyPropertyChanged
    {
        public HVACSupplyChannel() : base()
        {
            
            HVACObjectsList.Add(new HVACFilter());
            HVACObjectsList.Add(new HVACInletExchange());
            HVACObjectsList.Add(new HVACHeater());
            HVACObjectsList.Add(new HVACCooler());
            HVACObjectsList.Add(new HVACFan());
            HVACObjectsList.Add(new HVACFilter());
            SubscribeToAllItems();
        } 

        public void UpdateParams()
        {
            foreach (HVACObject obj in HVACObjectsList)
            {
                if(obj is IDynamicObject)
                {
                    ((IDynamicObject)obj).UpdateParams();
                }
            }
            CalculateDropAndFlow();
        }

        public void SetSpeedFan(double speed)
        {
            foreach(HVACObject obj in HVACObjectsList)
            {
                if(obj is HVACFan)
                {
                    ((HVACFan)obj).SetSpeedPercent = speed;
                }
            }
        }

        public void SetHeaterWaterTemperature(double temperature)
        {
            foreach(HVACObject obj in HVACObjectsList)
            {
                if(obj is HVACHeater)
                {
                    ((HVACHeater)obj).SetHotWaterTemperature = temperature;
                }
            }
        }

        public void SetHotWaterFlow(double flow)
        {
            foreach (HVACObject obj in HVACObjectsList)
            {
                if (obj is HVACHeater)
                {
                    ((HVACHeater)obj).HotWaterFlowPercent = flow;
                }
            }
        }
    }
}
