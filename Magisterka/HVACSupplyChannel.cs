using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace HVACSimulator
{
    public sealed class HVACSupplyChannel : AirChannel, IDynamicObject, INotifyPropertyChanged
    {
        public HVACSupplyChannel() : base()
        {
            InputAir = new Air(5, 40, EAirHum.relative);
            HVACObjectsList.Add(new HVACFilter());
            HVACObjectsList.Add(new HVACInletExchange());
            HVACObjectsList.Add(new HVACMixingBox(true));
            HVACObjectsList.Add(new HVACHeater());
            HVACObjectsList.Add(new HVACCooler());
            HVACObjectsList.Add(new HVACFan());
            HVACObjectsList.Add(new HVACFilter());
            SubscribeToAllItems();
            Name = "Kanał nawiewny";
            InitializePlotDataList();

        } 

        public HVACInletExchange GetInletExchange()
        {
            foreach(HVACObject obj in HVACObjectsList)
            {
                if (obj is HVACInletExchange) return (HVACInletExchange)obj;
            }
            throw new Exception("Brak wlotu powietrza wymiennika w kanale nawiewnym");
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
                    ((HVACHeater)obj).SetWaterTemperature = temperature;
                }
            }
        }

        public void SetHotWaterFlow(double flow)
        {
            foreach (HVACObject obj in HVACObjectsList)
            {
                if (obj is HVACHeater)
                {
                    ((HVACHeater)obj).WaterFlowPercent = flow;
                }
            }
        }

        public void SetCoolerWaterTemperature(double temperature)
        {
            foreach(HVACObject obj in HVACObjectsList)
            {
                if(obj is HVACCooler)
                {
                    ((HVACCooler)obj).SetWaterTemperature = temperature;
                }
            }
        }

        public void SetColdWaterFlow(double flow)
        {
            foreach (HVACObject obj in HVACObjectsList)
            {
                if (obj is HVACCooler)
                {
                    ((HVACCooler)obj).WaterFlowPercent = flow;
                }
            }
        }

        protected override void InitializePlotDataList()
        {
            PlotData tempPlotData = new PlotData(EDataType.flowRate, "Czas [s]", "Natężenie przepływu [m3/s]", Name); //TODO stopnie
            PlotDataList.Add(tempPlotData);
            PlotData humidPlotData = new PlotData(EDataType.pressureDrop, "Czas [s]", "Spadek ciśnienia [Pa]", Name);
            PlotDataList.Add(humidPlotData);
        }
    }
}
