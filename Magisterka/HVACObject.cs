using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka
{
    public abstract class HVACObject
    {
        public bool IsGenerativeFlow { get; set; }
        public double ACoeff { get; set; }
        public double BCoeff { get; set; }
        public double CCoeff { get; set; }
        public bool IsPresent { get; set; }
        public string Name { get; set; }

    }
}
