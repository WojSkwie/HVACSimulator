using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class HVACCooler : HVACTemperatureActiveObject, IDynamicObject
    {
       
        public HVACCooler() : base()
        {
            IsGenerativeFlow = false;
            Name = "Chłodnica";
            IsMovable = true;

            ACoeff = 1;
            BCoeff = 1;
            CCoeff = 0;

            TimeConstant = 1;

            SetWaterTemperature = 5;
            ActualWaterTemperature = 5;
            WaterFlowPercent = 100;
            HeatExchangeSurface = 1;
            HeatTransferCoeff = 200;
            MaximalWaterFlow = 1;

            ImageSource = @"images\cooler.png";

            SetPlotDataNames();
        }

        public override Air CalculateOutputAirParameters(Air inputAir, double airFlow)
        {
            double dewPoint = MolierCalculations.CalculateDewPoint(inputAir);

            double massAirFlow = MolierCalculations.FindAirDensity(inputAir.Temperature) * airFlow;
            double W1 = WaterFlowPercent * MaximalWaterFlow / 100 * Constants.heaterFluidHeatCapacity;
            double W2 = massAirFlow * Constants.airHeatCapacity;
            double Nominator = 1 - Math.Exp(-(1 - W1 / W2) * HeatTransferCoeff * HeatExchangeSurface / W1);
            double Denominator = 1 - W1 / W2 * Math.Exp(-(1 - W1 / W2) * HeatTransferCoeff * HeatExchangeSurface / W1);
            double Phi = Nominator / Denominator;
            double outputTemperatureKelvin = (inputAir.Temperature - 273) + (ActualWaterTemperature - inputAir.Temperature) * W1 / W2 * Phi;
            double outputTemperature = outputTemperatureKelvin + 273;

            if (dewPoint > ActualWaterTemperature) 
            {
                ///wykroplenie z wykorzystaniem remapowania 
                Air airAtCoolerDewPoint = new Air(ActualWaterTemperature, 100, EAirHum.relative);
                double oldRange = inputAir.Temperature - airAtCoolerDewPoint.Temperature;
                double newRange = inputAir.SpecificHumidity - airAtCoolerDewPoint.SpecificHumidity;
                double newSpecificHumidity = (((outputTemperature - airAtCoolerDewPoint.Temperature) * newRange) / oldRange) + airAtCoolerDewPoint.SpecificHumidity;
                OutputAir = new Air(outputTemperature, newSpecificHumidity, EAirHum.specific);
                //return outputAir; //TODO jezeli wilgotność ponad 100%
            }
            else
            {
                ///bez wykroplenia
                OutputAir = new Air(outputTemperature, inputAir.SpecificHumidity, EAirHum.specific);
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
        }
    }
}
