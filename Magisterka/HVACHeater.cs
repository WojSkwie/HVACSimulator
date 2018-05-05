using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public sealed class HVACHeater : HVACTemperatureActiveObject, IDynamicObject
    {
        public HVACHeater() : base()
        {
            IsGenerativeFlow = false;
            Name = "Nagrzewnica";
            IsMovable = true;

            HasSingleTimeConstant = true;

            ACoeff = 1;
            BCoeff = 1;
            CCoeff = 0;
            TimeConstant = 5;
            SetWaterTemperature = 80;
            ActualWaterTemperature = 80;
            WaterFlowPercent = 100;
            HeatExchangeSurface = 1;
            HeatTransferCoeff = 200;
            MaximalWaterFlow = 1;

            ImageSource = @"images\heater.png";

            SetPlotDataNames();
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

        public override Air CalculateOutputAirParameters(Air inputAir, double airFlow)
        {
            double massAirFlow = MolierCalculations.FindAirDensity(inputAir.Temperature) * airFlow;
            double W1 = WaterFlowPercent * MaximalWaterFlow / 100 * Constants.heaterFluidHeatCapacity;
            double W2 = massAirFlow * Constants.airHeatCapacity;
            double Nominator = 1 - Math.Exp(-(1 - W1 / W2) * HeatTransferCoeff * HeatExchangeSurface / W1);
            double Denominator = 1 - W1 / W2 * Math.Exp(-(1 - W1 / W2) * HeatTransferCoeff * HeatExchangeSurface / W1);
            double Phi = Nominator / Denominator;
            double outputTemperatureKelvin = (inputAir.Temperature - 273) + (ActualWaterTemperature - inputAir.Temperature) * W1 / W2 * Phi;
            OutputAir = new Air(outputTemperatureKelvin + 273, inputAir.SpecificHumidity, EAirHum.specific);
            AddDataPointFromAir(OutputAir, EDataType.humidity);
            AddDataPointFromAir(OutputAir, EDataType.temperature);
            return OutputAir;
        }

        
    }
}
