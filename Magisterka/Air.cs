using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka
{
    public enum EAirHum
    {
        relative,
        specific
    }
    public class Air : INotifyErrorSimulation, ICloneable
    {
        public double Temperature { get; set; }
        private double _SpecificHumidity;
        public double SpecificHumidity
        {
            get { return _SpecificHumidity; }
            set
            {
                _SpecificHumidity = value;
                _RelativeHumidity = MolierCalculations.HumiditySpecificToRelative(this);
            }
        }

        private double _RelativeHumidity;

        public event EventHandler<string> SimulationErrorOccured;

        public double RelativeHumidity
        {
            get { return _RelativeHumidity; }
            set
            {
                _RelativeHumidity = value;
                _SpecificHumidity = MolierCalculations.HumidityRelativeToSpecific(this);
            }
        }


        public Air(double temperature, double humidity, EAirHum airHum)
        {
            Temperature = temperature;
            switch (airHum)
            {
                case EAirHum.specific:
                    SpecificHumidity = humidity;
                    break;
                case EAirHum.relative:
                    RelativeHumidity = humidity;
                    break;
            }
        }

        public void OnSimulationErrorOccured(string error)
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            return new Air(Temperature, SpecificHumidity, EAirHum.specific);
            
        }
    }
}
