using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka
{
    public sealed class HVACFilter : HVACObject
    {
        public HVACFilter()
        {
            IsGenerativeFlow = false;
            Name = "Filtr";
        }
    }
}
