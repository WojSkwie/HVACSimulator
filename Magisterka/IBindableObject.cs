using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    
    public interface IBindableObject
    {
        double Min { get; set; }
        double Max { get; set; }
        int Index { get; set; }

        void InitializeParameters(params int[] indices);
    }

    
}
