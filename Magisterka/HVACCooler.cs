using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class HVACCooler : HVACTemperatureActiveObject, IDynamicObject
    {
        
        private const double ReferenceTemperatureDifference = 32 - 6;
        public double ActualCoolingPower { get; set; } 
        public double SetCoolingPower { get; set; }
        public double CoolingTimeConstant { get; set; }

        public HVACCooler() : base()
        {
            IsGenerativeFlow = false;
            Name = "Chłodnica";
            IsMovable = true;

            ACoeff = 1;
            BCoeff = 1;
            CCoeff = 0;

            TimeConstant = 1;

            SetWaterTemperature = 6;
            ActualWaterTemperature = 6;
            WaterFlowPercent = 100;
            //HeatExchangeSurface = 1;
            //HeatTransferCoeff = 200;
            ActualCoolingPower = 0;
            SetCoolingPower = 100;
            CoolingTimeConstant = 10;
            MaximalWaterFlow = 1;

            ImageSource = @"images\cooler.png";

            SetPlotDataNames();
        }

        public override Air CalculateOutputAirParameters(Air inputAir, double airFlow)
        {
            double massAirFlow = MolierCalculations.FindAirDensity(inputAir.Temperature) * airFlow;
            double inputAirDewPoint = MolierCalculations.CalculateDewPoint(inputAir);
            //Air airAtCoolerDewPoint = new Air(ActualWaterTemperature, 100, EAirHum.relative);
            double enthalpyDiff = 0;
            double energyDiff = 0;

            if (ActualWaterTemperature < inputAirDewPoint) ///wykroplenie
            {
                Air MaximallyCooledAir = new Air(ActualWaterTemperature, 100, EAirHum.relative);
                enthalpyDiff = inputAir.Enthalpy - MaximallyCooledAir.Enthalpy;
                energyDiff = enthalpyDiff * massAirFlow;
                if (ActualCoolingPower > energyDiff)
                {
                    OutputAir = MaximallyCooledAir;
                }
                else
                {
                    double temp = ActualWaterTemperature + (inputAir.Temperature - ActualWaterTemperature)
                        * ((energyDiff - ActualCoolingPower) / energyDiff);
                    double specHum = MaximallyCooledAir.SpecificHumidity + (inputAir.SpecificHumidity - MaximallyCooledAir.SpecificHumidity)
                        * ((energyDiff - ActualCoolingPower) / energyDiff);
                    OutputAir = new Air(temp, specHum, EAirHum.specific);
                }
            }
            else ///bez wykroplenia
            {
                Air MaximallyCooledAir = new Air(ActualWaterTemperature, inputAir.SpecificHumidity, EAirHum.specific);
                enthalpyDiff = inputAir.Enthalpy - MaximallyCooledAir.Enthalpy;
                energyDiff = enthalpyDiff * massAirFlow;
                if(ActualCoolingPower > energyDiff)
                {
                    OutputAir = MaximallyCooledAir;
                }
                else
                {
                    double temp = ActualWaterTemperature + (inputAir.Temperature - ActualWaterTemperature)
                        * ((energyDiff - ActualCoolingPower) / energyDiff);
                    OutputAir = new Air(temp, inputAir.SpecificHumidity, EAirHum.specific);
                    
                }

            }
            AddDataPointFromAir(OutputAir, EDataType.humidity);
            AddDataPointFromAir(OutputAir, EDataType.temperature);
            return OutputAir;


        }

        public void UpdateParams()
        {
            if (TimeConstant <= 0)
            {
                OnSimulationErrorOccured("Nieprawidłowa stała czasowa");
                return;
            }
            double derivative = (SetWaterTemperature - ActualWaterTemperature) / TimeConstant;
            ActualWaterTemperature += derivative * Constants.step;

            derivative = (SetCoolingPower - ActualCoolingPower) / CoolingTimeConstant;
            ActualCoolingPower += derivative * Constants.step;
        }
    }
}
