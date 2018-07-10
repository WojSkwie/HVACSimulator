﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public sealed class HVACCooler : HVACTemperatureActiveObject, IDynamicObject
    {
        
        private const double ReferenceTemperatureDifference = 32 - 6;
        public double ActualMaximalCoolingPower { get; set; } 
        public double SetMaximalCoolingPower { get; set; }
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
            ActualMaximalCoolingPower = 100;
            SetMaximalCoolingPower = 100;
            CoolingTimeConstant = 10;
            MaximalWaterFlow = 1;

            ImageSource = @"images\cooler.png";

            SetPlotDataNames();
        }

        public override Air CalculateOutputAirParameters(Air inputAir, double airFlow)
        {
            double massAirFlow = MolierCalculations.FindAirDensity(inputAir.Temperature) * airFlow;
            double inputAirDewPoint = MolierCalculations.CalculateDewPoint(inputAir);
            double enthalpyDiff = 0;
            double energyDiff = 0;
            Air MaximallyCooledAir;
            if (ActualWaterTemperature < inputAirDewPoint) ///wykroplenie
            {
                MaximallyCooledAir = new Air(ActualWaterTemperature, 100, EAirHum.relative);
            }
            else
            {
                MaximallyCooledAir = new Air(ActualWaterTemperature, inputAir.SpecificHumidity, EAirHum.specific);
            }
            enthalpyDiff = inputAir.Enthalpy - MaximallyCooledAir.Enthalpy;
            energyDiff = enthalpyDiff * massAirFlow;
            double coolingPower = ActualMaximalCoolingPower * (inputAir.Temperature - ActualWaterTemperature) / ReferenceTemperatureDifference;
            coolingPower *= WaterFlowPercent / 100;
            if (coolingPower > energyDiff)
            {
                OutputAir = MaximallyCooledAir;
            }
            else
            {
                double temp = ActualWaterTemperature + (inputAir.Temperature - ActualWaterTemperature)
                        * ((energyDiff - coolingPower) / energyDiff);
                double specHum = MaximallyCooledAir.SpecificHumidity + (inputAir.SpecificHumidity - MaximallyCooledAir.SpecificHumidity)
                    * ((energyDiff - coolingPower) / energyDiff);
                if (OutputAir.RelativeHumidity > 100) OutputAir.RelativeHumidity = 100; //TODO sprawdzić jak częśto występuje i jaka wartość
                OutputAir = new Air(temp, specHum, EAirHum.specific);
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

            derivative = (SetMaximalCoolingPower - ActualMaximalCoolingPower) / CoolingTimeConstant;
            ActualMaximalCoolingPower += derivative * Constants.step;
        }
    }
}
