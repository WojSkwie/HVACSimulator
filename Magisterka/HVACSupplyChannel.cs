using MahApps.Metro.Controls;
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

            SetInitialValuesParameters();
            ResetableObjects.AddRange(HVACObjectsList);
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

        public void InitializeControlDataContexts(
            NumericUpDown fanNumeric,
            NumericUpDown coolerNumeric,
            NumericUpDown heaterNumeric,
            NumericUpDown mixingNumeric)
        {
            fanNumeric.DataContext = HVACObjectsList.First(item => item is HVACFan);
            coolerNumeric.DataContext = HVACObjectsList.First(item => item is HVACCooler);
            heaterNumeric.DataContext = HVACObjectsList.First(item => item is HVACHeater);
            mixingNumeric.DataContext = HVACObjectsList.First(item => item is HVACMixingBox);

        }
        protected override void InitializePlotDataList()
        {
            PlotData tempPlotData = new PlotData(EDataType.flowRate, "Czas [s]", "Natężenie przepływu [m3/s]", Name); //TODO stopnie
            PlotDataList.Add(tempPlotData);
            PlotData humidPlotData = new PlotData(EDataType.pressureDrop, "Czas [s]", "Spadek ciśnienia [Pa]", Name);
            PlotDataList.Add(humidPlotData);
        }

        public override void SetInitialValuesParameters()
        {
            base.SetInitialValuesParameters();
        }

        public void AddPointToSeries(double airFlow, double pressure)
        {
            FlowRate = airFlow;
            FanPressureDrop = pressure;

            PlotData plotData = PlotDataList.Single(item => item.DataType == EDataType.flowRate);
            DataPoint newPoint = new DataPoint(GlobalParameters.SimulationTime, FlowRate);
            plotData.AddPointWithEvent(newPoint);

            PlotData pressPlotData = PlotDataList.Single(item => item.DataType == EDataType.pressureDrop);
            DataPoint PresNewPoint = new DataPoint(GlobalParameters.SimulationTime, FanPressureDrop);
            pressPlotData.AddPointWithEvent(PresNewPoint);
        }
    }
}
