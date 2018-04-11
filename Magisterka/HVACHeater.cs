using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka
{
    public sealed class HVACHeater : HVACObject, IDynamicObject
    {
        public HVACHeater()
        {
            IsGenerativeFlow = false;
            Name = "Nagrzewnica";
            IsMovable = true;

            HasSingleTimeConstant = true;

            ACoeff = 1;
            BCoeff = 1;
            CCoeff = 0;
            TimeConstant = 5;
            SetHotWaterTemperature = 80;
            ActualHotWaterTemperature = 80;
            HotWaterFlowPercent = 100;
            HeatExchangeSurface = 1;
            HeatTransferCoeff = 200;
            MaximalHotWaterFlow = 1;

            ImageSource = @"images\heater.png";
        }

        public double HotWaterFlowPercent { get; set; }
        public double MaximalHotWaterFlow { get; set; }

        public double ActualHotWaterTemperature { get; set; }
        public double SetHotWaterTemperature { get; set; }
 

        public void UpdateParams()
        {
            if (TimeConstant <= 0)
            {
                OnSimulationErrorOccured("Nieprawidłowa stała czasowa");
                return;
            }
            double derivative = (SetHotWaterTemperature - ActualHotWaterTemperature) / TimeConstant;
            ActualHotWaterTemperature += derivative * Constants.step;
        }

        public override Air CalculateOutputAirParameters(Air inputAir, double airFlow)
        {
            double massAirFlow = MolierCalculations.FindAirDensity(inputAir.Temperature) * airFlow;
            double W1 = HotWaterFlowPercent * MaximalHotWaterFlow / 100 * Constants.heaterFluidHeatCapacity;
            double W2 = massAirFlow * Constants.airHeatCapacity;
            double Nominator = 1 - Math.Exp(-(1 - W1 / W2) * HeatTransferCoeff * HeatExchangeSurface / W1);
            double Denominator = 1 - W1 / W2 * Math.Exp(-(1 - W1 / W2) * HeatTransferCoeff * HeatExchangeSurface / W1);
            double Phi = Nominator / Denominator;
            double outputTemperature = (inputAir.Temperature - 273) + (ActualHotWaterTemperature - inputAir.Temperature) * W1 / W2 * Phi;
            Air outputAir = new Air(outputTemperature, inputAir.SpecificHumidity, EAirHum.specific);
            return outputAir;
            //return outputTemperature + 273;
            //return base.CalculateOutputAirParameters(inputAir);
        }

        /*public override double CalculateOutputTemperature(double inputTemperatureOfAir, double airFlow)
        {
            double massAirFlow = MolierCalculations.FindAirDensity(inputTemperatureOfAir) * airFlow;
            double W1 = HotWaterFlowPercent * MaximalHotWaterFlow / 100 * Constants.heaterFluidHeatCapacity;
            double W2 = massAirFlow * Constants.airHeatCapacity;
            double Nominator = 1 - Math.Exp(-(1 - W1 / W2) * HeatTransferCoeff * HeatExchangeSurface / W1);
            double Denominator = 1 - W1 / W2 * Math.Exp(-(1 - W1 / W2) * HeatTransferCoeff * HeatExchangeSurface / W1);
            double Phi = Nominator / Denominator;
            double outputTemperature = (inputTemperatureOfAir - 273) + (ActualHotWaterTemperature - inputTemperatureOfAir) * W1 / W2 * Phi;
            return outputTemperature + 273;
        }*/
    }
}
