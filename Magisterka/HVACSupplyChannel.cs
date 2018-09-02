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
    public sealed class HVACSupplyChannel : AirChannel, IDynamicObject, INotifyPropertyChanged, IBindableAnalogOutput
    {
        public List<BindableAnalogOutputPort> BindedOutputs { get; set; }
        public Air OutputAir { get; set; }

        public HVACSupplyChannel() : base()
        {
            HVACObjectsList.Add(new HVACFilter(inverted: false));
            HVACObjectsList.Add(new HVACInletExchange());
            HVACObjectsList.Add(new HVACMixingBox(true));
            HVACObjectsList.Add(new HVACHeater());
            HVACObjectsList.Add(new HVACCooler());
            FanInChannel = new HVACFan(inverted: false);
            HVACObjectsList.Add(FanInChannel);
            HVACObjectsList.Add(new HVACFilter(inverted: false));
            SubscribeToAllItems();
            Name = "Kanał nawiewny";
            InitializePlotDataList();

            SetInitialValuesParameters();
            ResetableObjects.AddRange(HVACObjectsList);
            InitializeParametersList();
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
            OutputAir = new Air(20, 40, EAirHum.relative);
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

        public double CalculateDerivative(EVariableName variableName, double variableToDerivate)
        {
            throw new NotImplementedException(); //It stays like this ;)
        }

        public void InitializeParametersList()
        {
            BindedOutputs = new List<BindableAnalogOutputPort>
            {
                new BindableAnalogOutputPort(40,-20, true, EAnalogOutput.supplyAirTemperature)
            };

        }

        public List<EAnalogOutput> GetListOfParams(bool onlyVisible)// TODO
        {
            if(onlyVisible) return BindedOutputs.Where(item => item.Visibility == true).Select(item => item.AnalogOutput).ToList();
            return BindedOutputs.Select(item => item.AnalogOutput).ToList();
        }

        public int GetParameter(EAnalogOutput analogOutput)
        {
            var bindedParameter = BindedOutputs.FirstOrDefault(item => item.AnalogOutput == analogOutput);
            if (bindedParameter == null)
            {
                OnSimulationErrorOccured(string.Format("Próba odczytu nieprawidłowego parametru z kanału nawiewnego: {0}", analogOutput.ToString()));
                return 0;
            }
            int output = 0;
            switch (analogOutput)
            {
                case EAnalogOutput.supplyAirTemperature:
                    output = bindedParameter.ConvertTo12BitRange(OutputAir.Temperature);
                    break;
            }
            return output;
        }

        public override Air CalculateAirParametersWithAndAfterExchanger(Air InputAir, double airFlow, double massFlow)
        {
            OutputAir = base.CalculateAirParametersWithAndAfterExchanger(InputAir, airFlow, massFlow);
            return OutputAir;
        }

        public void DeactivateOutput(EAnalogOutput analogOutput)
        {
            BindedOutputs.Where(item => item.AnalogOutput.ToString() == analogOutput.ToString()).Single().Visibility = false;
        }
    }
}
