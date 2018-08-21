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
    public class ExchangerViewModel :IDynamicObject, INotifyPropertyChanged
    {
        
        
        public HVACSupplyChannel SupplyChannel { get; private set; }
        public HVACExhaustChannel ExhaustChannel { get; private set; }
        public List<Image> ImagesSupplyChannnel { get; private set; }
        public List<Image> ImagesExhaustChannel { get; private set; }
        public HVACExchanger Exchanger { get; private set; }
        public HVACEnvironment Environment { get; private set; }
        public HVACRoom Room { get; private set; }

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
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
    }
}
