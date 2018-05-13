using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public enum EAirHum
    {
        relative,
        specific
    }
    public class Air : INotifyErrorSimulation, ICloneable
    {
        private double _Temperature;
        public double Temperature
        {
            get { return _Temperature; }
            set
            {
                _Temperature = value;
                Enthalpy = MolierCalculations.CalculateEnthalpy(this);
            }
        }
        private double _SpecificHumidity;
        public double SpecificHumidity
        {
            get { return _SpecificHumidity; }
            set
            {
                _SpecificHumidity = value;
                _RelativeHumidity = MolierCalculations.HumiditySpecificToRelative(this);
                Enthalpy = MolierCalculations.CalculateEnthalpy(this);
            }
        }
        private double _RelativeHumidity;
        public double RelativeHumidity
        {
            get { return _RelativeHumidity; }
            set
            {
                _RelativeHumidity = value;
                _SpecificHumidity = MolierCalculations.HumidityRelativeToSpecific(this);
            }
        }
        public double Enthalpy { get; set; }

        public event EventHandler<string> SimulationErrorOccured;


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

        private Air()
        {

        }

        public void OnSimulationErrorOccured(string error)
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            Air clone = new Air
            {
                Temperature = this.Temperature,
                _RelativeHumidity = this.RelativeHumidity,
                _SpecificHumidity = this.SpecificHumidity
            };
            return clone;
            
        }
    }
}
