﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HVACSimulator
{
    public sealed class HVACFan : HVACObject, IDynamicObject, IBindableInput
    {
        public HVACFan() : base()
        {
            IsGenerativeFlow = true;
            Name = "Wentylator";
            IsMovable = true;
            IsMutable = false;

            ActualSpeedPercent = 0.01;
            SetSpeedPercent = 0.01;
            TimeConstant = 5;
            HasSingleTimeConstant = true;

            ACoeff = -1;
            BCoeff = 1;
            CCoeff = 120;

            ImageSource = @"images\fan.png";
            SetPlotDataNames();
        } 
        public double SetSpeedPercent { get; set; } 
        public double ActualSpeedPercent { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public int Index { get; set; }

        public void InitializeParameters(params int[] indices)
        {
            Index = 0;
        }

        public void SetParameter(double parameter)
        {
            throw new NotImplementedException();
        }

        public void UpdateParams()
        {
            if(TimeConstant <= 0 )
            {
                OnSimulationErrorOccured("Nieprawidłowa stała czasowa");
                return;
            }
            double derivative = (SetSpeedPercent - ActualSpeedPercent) / TimeConstant;
            ActualSpeedPercent += derivative * Constants.step;

        }
    }
}
