using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HVACSimulator
{
    public sealed class HVACFilter : HVACObject
    {
        public HVACFilter(bool inverted) : base()
        {
            IsGenerativeFlow = false;
            Name = "Filtr";
            IsMovable = false;

            if(inverted)
            {
                ImageSource = @"images\filter2.png";
            }
            else
            {
                ImageSource = @"images\filter1.png";
            }

            SetInitialValuesParameters();
            SetPlotDataNames();
        }
        public override void SetInitialValuesParameters()
        {
            base.SetInitialValuesParameters();
            ACoeff = 400;
            BCoeff = 0;
            CCoeff = 0;
        }

    }
}
