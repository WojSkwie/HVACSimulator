using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka
{
    public abstract class AirChannel : INotifyErrorSimulation, INotifyPropertyChanged
    {
        public ObservableCollection<HVACObject> HVACObjectsList { get; set; } = new ObservableCollection<HVACObject>();

        protected AirChannel()
        {
        }

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
            OnChannelPresenceChanged();
        }

        protected void CalculateDropAndFlow()
        {
            double A = 0, B = 0, C = 0;
            double Ap = 0, Bp = 0, Cp = 0;
            foreach (HVACObject obj in HVACObjectsList)
            {
                if(!obj.IsPresent) { continue; }
                if(obj.IsGenerativeFlow)
                {
                    if(obj is HVACFan)
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

            double delta = MyMath.CalculateDelta(A, B, C);
            if(delta < 0 ) { OnSimulationErrorOccured("Charakterystyki nie mają punktu wspólnego"); }
            double[] roots = MyMath.FindRoots(A, B, C, delta);
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
            double pressure = MyMath.QuadEquaVal(Ap, Bp, Cp, flow);

            FlowRate = flow;
            FanPressureDrop = pressure;

        }

        public void OnSimulationErrorOccured(string error)
        {
            //throw new NotImplementedException();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
