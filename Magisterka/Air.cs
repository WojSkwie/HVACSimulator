﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class Air : INotifyErrorSimulation, ICloneable, INotifyPropertyChanged
    {
        private double _Temperature;
        public double Temperature
        {
            get { return _Temperature; }
            set
            {
                _Temperature = value;
                Enthalpy = MolierCalculations.CalculateEnthalpy(this);
                OnPropertyChanged("Temperature");
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
                OnPropertyChanged("SpecificHumidity");
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
                Enthalpy = MolierCalculations.CalculateEnthalpy(this);
                OnPropertyChanged("RelativeHumidity");
            }
        }
        public double Enthalpy { get; private set; }

        public event EventHandler<string> SimulationErrorOccured;
        public event PropertyChangedEventHandler PropertyChanged;

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
            GetGlobalErrorHandlerSubscription();
        }

        private Air()
        {
            GetGlobalErrorHandlerSubscription();
        }

        public void OnSimulationErrorOccured(string error)
        {
            SimulationErrorOccured?.Invoke(this, error);
        }

        public object Clone()
        {
            Air clone = new Air
            {
                Temperature = this.Temperature,
                //RelativeHumidity = this.RelativeHumidity,
                SpecificHumidity = this.SpecificHumidity
            };
            return clone;
            
        }

        public void GetGlobalErrorHandlerSubscription()
        {
            SimulationErrorOccured += GlobalParameters.Instance.OnErrorSimulationOccured;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
