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
        public HVACFilter() : base()
        {
            IsGenerativeFlow = false;
            Name = "Filtr";
            IsMovable = false;


            ImageSource = @"images\filter.png";

            SetInitialValuesParameters();
            SetPlotDataNames();
        }
        public override void SetInitialValuesParameters()
        {
            base.SetInitialValuesParameters();
            ACoeff = 1;
            BCoeff = 1;
            CCoeff = 0;
        }

    }
}
