using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka
{
    public class HVACCooler : HVACTemperatureActiveObject, IDynamicObject
    {
        /*public double SurfaceTemperature { get; set; }

        public double HotWaterFlowPercent { get; set; }
        public double MaximalHotWaterFlow { get; set; }

        public double ActualHotWaterTemperature { get; set; }
        public double SetHotWaterTemperature { get; set; }*/


        public HVACCooler()
        {
            IsGenerativeFlow = false;
            Name = "Chłodnica";
            IsMovable = true;

            ACoeff = 1;
            BCoeff = 1;
            CCoeff = 0;

            //SurfaceTemperature = 5;

            ImageSource = @"images\cooler.png";
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
            double outputTemperature = (inputAir.Temperature - 273) + (ActualWaterTemperature - inputAir.Temperature) * W1 / W2 * Phi;
            Air outputAir = new Air(outputTemperature, inputAir.SpecificHumidity, EAirHum.specific);
            

            if (dewPoint > ActualWaterTemperature) 
            {//wykroplenie

            }
            else
            {//bez wykroplenia
                
            }
        }

        public void UpdateParams()
        {
            Console.Write("TODO UPDATE PARAMS COOLER\n"); //TODO
        }
    }
}
