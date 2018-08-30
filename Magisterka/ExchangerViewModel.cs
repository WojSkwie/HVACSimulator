using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HVACSimulator
{
    public class ExchangerViewModel :IDynamicObject, INotifyPropertyChanged, IResetableObject, INotifyErrorSimulation
    {
        
        
        public HVACSupplyChannel SupplyChannel { get; private set; }
        public HVACExhaustChannel ExhaustChannel { get; private set; }
        public List<Image> ImagesSupplyChannnel { get; private set; }
        public List<Image> ImagesExhaustChannel { get; private set; }
        public HVACExchanger Exchanger { get; private set; }
        public HVACEnvironment Environment { get; private set; }
        public HVACRoom Room { get; private set; }
        private List<IResetableObject> ResetableObjects = new List<IResetableObject>();

        private bool _AllowChanges = true;

        public bool AllowChanges
        {
            get { return _AllowChanges; }
            set
            {
                _AllowChanges = value;
                OnPropertyChanged("AllowChanges");
            }
        }

        public ExchangerViewModel()
        {
            SupplyChannel = new HVACSupplyChannel();
            ExhaustChannel = new HVACExhaustChannel();
            ImagesSupplyChannnel = new List<Image>();
            ImagesExhaustChannel = new List<Image>();
            Environment = new HVACEnvironment();
            Room = new HVACRoom(Environment);
            SupplyChannel.ImagesList = ImagesSupplyChannnel;
            ExhaustChannel.ImagesList = ImagesExhaustChannel;
            Exchanger = new HVACExchanger(SupplyChannel.GetInletExchange(), ExhaustChannel.GetOutletExchange());
            CoupleMixingBoxes();
            CoupleFans();

            ResetableObjects.Add(SupplyChannel);
            ResetableObjects.Add(ExhaustChannel);
            ResetableObjects.Add(Environment);
            ResetableObjects.Add(Room);
            ResetableObjects.Add(Exchanger);

            GetGlobalErrorHandlerSubscription();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<string> SimulationErrorOccured;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void CoupleMixingBoxes()
        {
            HVACMixingBox first = SupplyChannel.GetMixingBox();
            HVACMixingBox second = ExhaustChannel.GetMixingBox();
            first.CoupleMixingBox(second);
            second.CoupleMixingBox(first);
        }

        private void CoupleFans()
        {
            HVACFan first = SupplyChannel.FanInChannel;
            HVACFan second = ExhaustChannel.FanInChannel;
            first.CoupleFan(second);
            second.CoupleFan(first);
        }

        public void UpdateParams()
        {
            SupplyChannel.UpdateParams();
            ExhaustChannel.UpdateParams();
            ExhaustChannel.FlowRate = SupplyChannel.FlowRate;
            Exchanger.UpdateSetEfficiency(GetFlowRateFromSupplyChannel());
            Exchanger.UpdateParams();
        }

        public void InitializeDataContextsForControlNumerics(
            NumericUpDown fanNumeric,
            NumericUpDown coolerNumeric,
            NumericUpDown heaterNumeric,
            NumericUpDown mixingNumeric)
        {
            SupplyChannel.InitializeControlDataContexts(fanNumeric, coolerNumeric, heaterNumeric, mixingNumeric);
            
        }

        public double GetSpeedFromSupplyChannel()
        {
            foreach (HVACObject obj in SupplyChannel.HVACObjectsList)
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
            foreach (HVACObject obj in SupplyChannel.HVACObjectsList)
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
            foreach (HVACObject obj in SupplyChannel.HVACObjectsList)
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
            return SupplyChannel.FlowRate;
        }

        public double GetPressureDropFromSupplyChannel()
        {
            return SupplyChannel.FanPressureDrop;
        }

        public void SetInitialValuesParameters()
        {
            foreach(IResetableObject resetableObject in ResetableObjects)
            {
                resetableObject.SetInitialValuesParameters();
            }

            AllowChanges = true;
        }


        public void CalculateAirFlowInChannels(out double airFlow, out double massFlow, out double pressure, Air EnviromentAir)
        {
            double A = 0, B = 0, C = 0, Ap = 0, Bp = 0, Cp = 0;
            SupplyChannel.GatherParametersFromObjects(ref A, ref B, ref C, ref Ap, ref Bp, ref Cp);
            ExhaustChannel.GatherParametersFromObjects(ref A, ref B, ref C, ref Ap, ref Bp, ref Cp);

            double delta = MathUtil.CalculateDelta(A, B, C);
            if (delta < 0) { OnSimulationErrorOccured("Charakterystyki nie mają punktu wspólnego"); }
            double[] roots = MathUtil.FindRoots(A, B, C, delta);
            if (roots[0] > 0)
            {
                airFlow = roots[0];
            }
            else if (roots[1] > 0)
            {
                airFlow = roots[1];
            }
            else
            {
                OnSimulationErrorOccured("Charakterystyki nie mają dodatniego punktu wspólnego");
                airFlow = 0; massFlow = 0; pressure = 0;
                return;
            }
            pressure = MathUtil.QuadEquaVal(Ap, Bp, Cp, airFlow);
            if (pressure < 0)
            {
                OnSimulationErrorOccured("Ujemna wartość spadku ciśnienia");
                airFlow = 0; massFlow = 0; pressure = 0;
                return;
            }
            double density = MolierCalculations.FindAirDensity(EnviromentAir);
            massFlow = density * airFlow;

            SupplyChannel.AddPointToSeries(airFlow, pressure);
        }

        public void GetGlobalErrorHandlerSubscription()
        {
            SimulationErrorOccured += GlobalParameters.Instance.OnErrorSimulationOccured;
        }

        public void OnSimulationErrorOccured(string error)
        {
            SimulationErrorOccured?.Invoke(this, error);
        }

        public double CalculateDerivative(EVariableName variableName, double variableToDerivate)
        {
            throw new NotImplementedException();
        }
    }
}
