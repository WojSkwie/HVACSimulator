using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HVACSimulator
{
    public abstract class AirChannel : PlottableObject, INotifyErrorSimulation, INotifyPropertyChanged, IModifiableCharact, IResetableObject
    {
        public ObservableCollection<HVACObject> HVACObjectsList { get; set; } = new ObservableCollection<HVACObject>();

        protected AirChannel() : base()
        {
            GlobalParameters = GlobalParameters.Instance;
            GetSubscription();
        }

        protected List<IResetableObject> ResetableObjects = new List<IResetableObject>();

        protected GlobalParameters GlobalParameters;
        protected double _FlowRate;
        protected double _FanPressureDrop;

        public double FlowRate
        {
            get
            {
                return _FlowRate;
            }
            set
            {
                if(_FlowRate != value)
                {
                    _FlowRate = value;
                    OnPropertyChanged("FlowRate");
                }
            }
        } 
        public double FanPressureDrop
        {
            get
            {
                return _FanPressureDrop;
            }
            set
            {
                if(_FanPressureDrop != value)
                {
                    _FanPressureDrop = value;
                    OnPropertyChanged("FanPressureDrop");
                }
            }
        }
        public double EmptyChannelPressureDrop { get; set; }
        //public Air InputAir { get; set; }
        public List<Image> ImagesList;
        public string Name { get; set; }

        protected void SubscribeToAllItems()
        {
            foreach (HVACObject obj in HVACObjectsList)
            {
                obj.PropertyChanged += PresenceChanged;
            }
        }

        public event EventHandler<EventArgs> ChannelPresenceChanged;
        public event EventHandler<string> SimulationErrorOccured;
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnChannelPresenceChanged()
        {
            ChannelPresenceChanged?.Invoke(this, EventArgs.Empty);
        }

        public void PresenceChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsPresent":
                    OnChannelPresenceChanged();
                    break;
                default:

                    break;
            }

            
        }

        protected void GatherParametersFromObjects(out double A, out double B, out double C, 
            out double Ap, out double Bp, out double Cp)
        {
            A = 0; B = 0; C = 0; Ap = 0; Bp = 0; Cp = 0;

            C += EmptyChannelPressureDrop;
            Cp += EmptyChannelPressureDrop;

            foreach (HVACObject obj in HVACObjectsList)
            {
                if (!obj.IsPresent) { continue; }
                if (obj.IsGenerativeFlow)
                {
                    if (obj is HVACFan)
                    {
                        double percent = ((HVACFan)obj).ActualSpeedPercent;
                        A -= obj.ACoeff * (percent / 100);
                        B -= obj.BCoeff * (percent / 100);
                        C -= obj.CCoeff * (percent / 100);
                    }
                }
                else
                {
                    A += obj.ACoeff;
                    B += obj.BCoeff;
                    C += obj.CCoeff;

                    Ap += obj.ACoeff;
                    Bp += obj.BCoeff;
                    Cp += obj.CCoeff;
                }
            }
        }

        protected void CalculateDropAndFlow()
        {
            GatherParametersFromObjects(
                out double A, out double B, out double C,
                out double Ap, out double Bp, out double Cp);

            double delta = MathUtil.CalculateDelta(A, B, C);
            if(delta < 0 ) { OnSimulationErrorOccured("Charakterystyki nie mają punktu wspólnego"); }
            double[] roots = MathUtil.FindRoots(A, B, C, delta);
            double flow;
            if(roots[0] > 0 )
            {
                flow = roots[0];
            }
            else if(roots[1] > 0)
            {
                flow = roots[1];
            }
            else
            {
                OnSimulationErrorOccured("Charakterystyki nie mają dodatniego punktu wspólnego");
                return;
            }
            double pressure = MathUtil.QuadEquaVal(Ap, Bp, Cp, flow);
            if(pressure < 0)
            {
                OnSimulationErrorOccured("Ujemna wartość spadku ciśnienia");
                return;
            }

            FlowRate = flow;
            FanPressureDrop = pressure;

            AddFlowDataFromAirParams();
            AddPressureDataFromAirParams();

        }

        public void OnSimulationErrorOccured(string error)
        {
            SimulationErrorOccured?.Invoke(this, error);
            //throw new NotImplementedException();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ModifyCharacteristics()
        {
            var dialog = new ChannelCharactWindow(this);
            dialog.ShowDialog();
        }

        /// <param name="direction">1 for moving down the list, 1 for up the list</param>
        public void MoveObject(int index, int direction)
        {
            if (index + direction > -1 && index + direction < HVACObjectsList.Count )
            {
                if (HVACObjectsList[index].IsMovable && HVACObjectsList[index + direction].IsMovable)
                {
                    HVACObject temp = HVACObjectsList[index + direction];
                    HVACObjectsList.RemoveAt(index + direction);
                    HVACObjectsList.Insert(index, temp);
                }
            }
        }

        /*public void CalculateAirParameters()
        {
            if (FlowRate == 0) return;
            Air air = InputAir;
            
            foreach(HVACObject obj in HVACObjectsList)
            {
                if (!obj.IsPresent) continue;
                air = obj.CalculateOutputAirParameters(air, FlowRate);
            }
        }*/

        public Air CalculateAirParametersBeforeExchanger(Air InputAir)
        {
            if (FlowRate == 0) return InputAir; //TODO tutaj nie wiem jeszcze jak powinien zareagować układ
            Air air = InputAir;
            foreach(HVACObject obj in HVACObjectsList)
            {
                if (obj is HVACInletExchange || obj is HVACOutletExchange) return air;
                air = obj.CalculateOutputAirParameters(air, FlowRate);
            }
            return air;
        }

        public Air CalculateAirParametersWithAndAfterExchanger(Air InputAir)
        {
            if (FlowRate == 0) return InputAir; //TODO tutaj nie wiem jeszcze jak powinien zareagować układ
            Air air = new Air(0, 0, EAirHum.relative); //powietrze słup. korzystam przez źle napisaną metode obliczającą
            bool foundExchanger = false;
            foreach(HVACObject obj in HVACObjectsList)
            {
                if (obj is HVACInletExchange || obj is HVACOutletExchange) { foundExchanger = true; }
                if (foundExchanger) air = obj.CalculateOutputAirParameters(air, FlowRate);
            }
            return air;
        }

        public HVACMixingBox GetMixingBox()
        {
            foreach(HVACObject obj in HVACObjectsList)
            {
                if (obj is HVACMixingBox) return (HVACMixingBox)obj;
            }
            throw new Exception("Brak komory mieszania w kanale");
        }

        protected void AddFlowDataFromAirParams()
        {
            PlotData plotData = PlotDataList.Single(item => item.DataType == EDataType.flowRate);
            DataPoint newPoint = new DataPoint(GlobalParameters.SimulationTime, FlowRate);
            plotData.AddPointWithEvent(newPoint);
        }

        protected void AddPressureDataFromAirParams()
        {
            PlotData plotData = PlotDataList.Single(item => item.DataType == EDataType.pressureDrop);
            DataPoint newPoint = new DataPoint(GlobalParameters.SimulationTime, FanPressureDrop);
            plotData.AddPointWithEvent(newPoint);
        }

        public override void SetInitialValuesParameters()
        {
            base.SetInitialValuesParameters();
            foreach (var resetableElement in ResetableObjects)
            {
                resetableElement.SetInitialValuesParameters();
            }
        }

        public void GetSubscription()
        {
            SimulationErrorOccured += GlobalParameters.Instance.OnErrorSimulationOccured;
        }
    }
}
