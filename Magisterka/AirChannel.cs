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
            GetGlobalErrorHandlerSubscription();
        }

        protected List<IResetableObject> ResetableObjects = new List<IResetableObject>();

        protected GlobalParameters GlobalParameters;
        protected double _FlowRate;
        protected double _FanPressureDrop;
        public HVACFan FanInChannel { get; protected set; }

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

        public void GatherParametersFromObjects(ref double A, ref double B, ref double C, 
            ref double Ap, ref double Bp, ref double Cp)
        {
            C += EmptyChannelPressureDrop;
            //Cp += EmptyChannelPressureDrop;

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

        public void OnSimulationErrorOccured(string error)
        {
            SimulationErrorOccured?.Invoke(this, error);
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

        public Air CalculateAirParametersBeforeExchanger(Air InputAir, ref double airFlow, ref double massFlow)
        {
            if (FlowRate == 0) return InputAir; //TODO tutaj nie wiem jeszcze jak powinien zareagować układ
            Air air = InputAir;
            foreach(HVACObject obj in HVACObjectsList)
            {
                if (obj is HVACInletExchange || obj is HVACOutletExchange) return air;
                air = obj.CalculateOutputAirParameters(air, ref airFlow, ref massFlow);
            }
            return air;
        }

        public virtual Air CalculateAirParametersWithAndAfterExchanger(Air InputAir, ref double airFlow, ref double massFlow)
        {
            if (FlowRate == 0) return InputAir; //TODO tutaj nie wiem jeszcze jak powinien zareagować układ
            Air air = new Air(0, 0, EAirHum.relative); //powietrze słup. korzystam przez źle napisaną metode obliczającą
            bool foundExchanger = false;
            //double airFlowInParticularObjects = FlowRate;
            foreach(HVACObject obj in HVACObjectsList)
            {
                if (obj is HVACInletExchange || obj is HVACOutletExchange) { foundExchanger = true; }
                if (foundExchanger) air = obj.CalculateOutputAirParameters(air, ref airFlow, ref massFlow);
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

        public override void SetInitialValuesParameters()
        {
            base.SetInitialValuesParameters();
            foreach (var resetableElement in ResetableObjects)
            {
                resetableElement.SetInitialValuesParameters();
            }
        }

        public void GetGlobalErrorHandlerSubscription()
        {
            SimulationErrorOccured += GlobalParameters.Instance.OnErrorSimulationOccured;
        }
    }
}
