using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HVACSimulator
{
    public class HVACExchanger : INotifyErrorSimulation, IDynamicObject, IBindableDigitalInput, IBindableDigitalOutput, IResetableObject
    {
        private HVACOutletExchange OutletExchange;
        private HVACInletExchange InletExchange;

        public double AproxA { get; set; }
        public double AproxB { get; set; }
        public double AproxC { get; set; }
        public double AproxD { get; set; }

        public double SetEfficiencyPercent { get; private set; }
        private double ActualEfficiencyPercent { get; set; }
        private const double ReferenceTemperatureDifference = 32 - 6; 

        public double TimeConstant;

        public event EventHandler<string> SimulationErrorOccured;

        public bool BypassActivated { get; private set; }
        public bool IsFrozen { get; private set; }

        public double SecondsToFreeze { get; set; }
        public double SecondsToMelt { get; set; }

        private double NegativeTempTime;
        private double PositiveTempTime;

        
        List<EDigitalInput> IBindableDigitalInput.ParamsList { get; set; } = new List<EDigitalInput>
        {
            EDigitalInput.bypass
        };
        List<EDigitalOutput> IBindableDigitalOutput.ParamsList { get; set; } = new List<EDigitalOutput>
        {
            EDigitalOutput.frozenExchanger
        };

        public HVACExchanger(HVACInletExchange inletExchange, HVACOutletExchange outletExchange)
        {
            InletExchange = inletExchange;
            OutletExchange = outletExchange;
            GetGlobalErrorHandlerSubscription();
            SetInitialValuesParameters();
        }
        /// <summary>
        /// Oblicza parametry powietrza na wylotach kanałów. Zakłada równy przepływ przez nawiew i wywiew
        /// </summary>
        public void CalculateExchangeAndSetOutputAir(Air supplyAir, Air exhaustAir, out Air supplyAirAfter, out Air exhaustAirAfter)
        {
            if(BypassActivated) 
            {
                CalculateAirParamsWhenBypassOn(supplyAir, exhaustAir, out supplyAirAfter, out exhaustAirAfter);
            }
            else
            {
                CalculateAirParamsWhenBypassOff(supplyAir, exhaustAir, out supplyAirAfter, out exhaustAirAfter);
            }
            CheckAndSetFreezingStatus(exhaustAir);
        }

        public void OnSimulationErrorOccured(string error)
        {
            SimulationErrorOccured?.Invoke(this, error);
            MessageBox.Show(error);
        }

        public double UpdateSetEfficiency(double airFlow)
        {
            double efficiency = MathUtil.QubicEquaVal(AproxA, AproxB, AproxC, AproxD, airFlow);
            if (efficiency > 95) efficiency = 95;
            if (efficiency < 0) efficiency = 0;
            SetEfficiencyPercent = efficiency;
            return efficiency;
        }

        void IBindableDigitalInput.SetDigitalParameter(bool state, EDigitalInput digitalInput)
        {
            switch (digitalInput)
            {
                case EDigitalInput.bypass:
                    BypassActivated = state;
                    break;
                default:
                    OnSimulationErrorOccured(string.Format("Próba przypisania nieprawidłowego parametru do wymiennika ciepła: {0}", digitalInput));
                    break;
            }

        }

        bool IBindableDigitalOutput.GetDigitalParameter(EDigitalOutput digitalOutput)
        {
            switch(digitalOutput)
            {
                case EDigitalOutput.frozenExchanger:
                    return !IsFrozen;
                default:
                    OnSimulationErrorOccured(string.Format("Próba odczytu nieprawidłowego parametru z wymiennika ciepła: {0}", digitalOutput));
                    return false;
            }
        }

        public void GetGlobalErrorHandlerSubscription()
        {
            SimulationErrorOccured += GlobalParameters.Instance.OnErrorSimulationOccured;
        }

        public void SetInitialValuesParameters()
        {
            TimeConstant = 2;
            AproxA = -11.37;
            AproxB = 53.21;
            AproxC = -68.65;
            AproxD = 97.93;

            SecondsToFreeze = 30;
            SecondsToMelt = 30;
            NegativeTempTime = 0;
            PositiveTempTime = 0;
        }

        public double CalculateDerivative(EVariableName variableName, double variableToDerivate)
        {
            switch (variableName)
            {
                case EVariableName.exchangerEfficiency:
                    return (SetEfficiencyPercent - variableToDerivate) / TimeConstant;
                default:
                    OnSimulationErrorOccured(string.Format("Próba całkowania niewłaściwego obiektu w wymienniku ciepła: {0}", variableName));
                    return 0;
            }
        }

        public void UpdateParams()
        {
            if (TimeConstant <= 0)
            {
                OnSimulationErrorOccured("Nieprawidłowa stała czasowa");
                return;
            }

            double startDerivative = CalculateDerivative(EVariableName.exchangerEfficiency, ActualEfficiencyPercent);
            double midValue = ActualEfficiencyPercent + (startDerivative * Constants.step / 2.0);
            double midDerivative = CalculateDerivative(EVariableName.exchangerEfficiency, midValue);
            ActualEfficiencyPercent += midDerivative * Constants.step;
        }

        private void CalculateAirParamsWhenBypassOn(Air supplyAir, Air exhaustAir, out Air supplyAirAfter, out Air exhaustAirAfter)
        {
            OutletExchange.OutputAir = (Air)exhaustAir.Clone();
            InletExchange.OutputAir = (Air)supplyAir.Clone();
            supplyAirAfter = InletExchange.OutputAir;
            exhaustAirAfter = OutletExchange.OutputAir;
        }

        private void CalculateAirParamsWhenBypassOff(Air supplyAir, Air exhaustAir, out Air supplyAirAfter, out Air exhaustAirAfter)
        {
            //Założenie -> równy przepływ powietrza z obu kanałach.
            Air heatingAir; Air coolingAir;
            if (supplyAir.Temperature > exhaustAir.Temperature)
            {
                heatingAir = supplyAir; coolingAir = exhaustAir;
            }
            else
            {
                heatingAir = exhaustAir; coolingAir = supplyAir;
            }
            double dewPoint = MolierCalculations.CalculateDewPoint(heatingAir);
            double tempDiff = heatingAir.Temperature - coolingAir.Temperature;
            double heatedtemp = coolingAir.Temperature + tempDiff * (ActualEfficiencyPercent / 100);
            Air heatedAir;
            Air cooledAir;
            heatedAir = new Air(heatedtemp, coolingAir.SpecificHumidity, EAirHum.specific);
            double enthalpyAdded = heatedAir.Enthalpy - coolingAir.Enthalpy;
            Air maximallyCooledAir;
            if (dewPoint > heatingAir.Temperature) //wykroplenie
            {
                maximallyCooledAir = new Air(coolingAir.Temperature, 100, EAirHum.relative);
            }
            else //bez wykroplenia
            {
                maximallyCooledAir = new Air(coolingAir.Temperature, heatingAir.SpecificHumidity, EAirHum.specific);
            }
            double maximumEnthalpyRemoved = heatingAir.Enthalpy - maximallyCooledAir.Enthalpy;
            double proportion = enthalpyAdded / maximumEnthalpyRemoved;
            if (proportion > 1 || proportion < 0) OnSimulationErrorOccured("Niewłaściwy stosunek energi w wymienniku");

            double tempCoolDiff = heatingAir.Temperature - maximallyCooledAir.Temperature;
            double humidCoolDiff = heatingAir.SpecificHumidity - maximallyCooledAir.SpecificHumidity;
            double cooledTemp = heatingAir.Temperature - tempCoolDiff * proportion;
            double cooledHumid = heatingAir.SpecificHumidity - humidCoolDiff * proportion;
            cooledAir = new Air(cooledTemp, cooledHumid, EAirHum.specific);
            if (supplyAir.Temperature > exhaustAir.Temperature)
            {
                supplyAirAfter = cooledAir; exhaustAirAfter = heatedAir;
            }
            else
            {
                supplyAirAfter = heatedAir; exhaustAirAfter = cooledAir;
            }
            OutletExchange.OutputAir = exhaustAirAfter;
            InletExchange.OutputAir = supplyAirAfter;
        }

        private void CheckAndSetFreezingStatus(Air exhaustAir)
        {
            if (IsFrozen)
            {
                if (exhaustAir.Temperature < 0)
                {
                    PositiveTempTime -= Constants.step;
                    NegativeTempTime = 0;
                }
                else
                {
                    PositiveTempTime += Constants.step;
                    NegativeTempTime = 0;
                    if (PositiveTempTime >= SecondsToMelt)
                    {
                        IsFrozen = false;
                        PositiveTempTime = 0;
                    }
                }
            }
            else
            {
                if (exhaustAir.Temperature < 0)
                {
                    NegativeTempTime += Constants.step;
                    PositiveTempTime = 0;
                    if (NegativeTempTime >= SecondsToFreeze)
                    {
                        IsFrozen = true;
                        NegativeTempTime = 0;
                    }
                }
                else
                {
                    PositiveTempTime = 0;
                    NegativeTempTime = 0;
                }
            }
        }
    }
}
