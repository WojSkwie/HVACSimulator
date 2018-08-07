using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public sealed class HVACMixingBox : HVACObject, IBindableAnalogInput
    {
        private HVACMixingBox CoupledMixingBox;
        private bool InSupply;
        /// <summary>
        /// Procent powietrza użytego ponownie
        /// </summary>
        public double MixingPercent { get; set; }
        public HVACMixingBox(bool inSupply)
        {
            IsGenerativeFlow = false;
            Name = "Komora mieszania";
            IsMovable = false;
            IsMutable = true;
            InSupply = inSupply;
            ImageSource = @"images\fan.png";
            SetPlotDataNames();
           
            MixingPercent = 100;
        }

        public override bool IsPresent
        {
            get => base.IsPresent;
            set
            {
                this._IsPresent = value;
                OnPropertyChanged("IsPresent");
                if (value != IsPresent)
                {
                    IsPresent = value;
                    if (CoupledMixingBox != null) 
                    {
                        CoupledMixingBox.IsPresent = value;
                    }
                }
            }
        }

        public List<int> AIIndices { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double Min { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double Max { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void CoupleMixingBox(HVACMixingBox mixingBox)
        {
            CoupledMixingBox = mixingBox;
        }

        public override Air CalculateOutputAirParameters(Air inputAir, double airFlow)
        {
            if(InSupply)
            {
                double specHum = (inputAir.SpecificHumidity * (100 - MixingPercent) +
                    CoupledMixingBox.OutputAir.SpecificHumidity * MixingPercent) / 100; //średnia ważona
                double temp = (inputAir.Temperature * (100 - MixingPercent) +
                    CoupledMixingBox.OutputAir.Temperature * MixingPercent) / 100; //średnia ważona
                OutputAir = new Air(temp, specHum, EAirHum.specific);
                AddDataPointFromAir(OutputAir, EDataType.temperature);
                AddDataPointFromAir(OutputAir, EDataType.humidity);
                return OutputAir;
            }
            else
            {
                return base.CalculateOutputAirParameters(inputAir, airFlow);
            }
        }

        public void SetParameter(int parameter, EAnalogInput analogInput)
        {
            throw new NotImplementedException();
        }

        public void InitializeParameters()
        {
            throw new NotImplementedException();
        }
    }
}
