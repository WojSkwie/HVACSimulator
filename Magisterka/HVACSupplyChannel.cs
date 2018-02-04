using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka
{
    public class HVACSupplyChannel : IDynamicObject
    {
        public List<HVACObject> HVACObjectsList = new List<HVACObject>();
        public HVACSupplyChannel()
        {
            HVACObjectsList.Add(new HVACFilter());
            HVACObjectsList.Add(new HVACHeater());
            HVACObjectsList.Add(new HVACCooler());
            HVACObjectsList.Add(new HVACFan());
            HVACObjectsList.Add(new HVACFilter());
        }

        public void UpdateParams()
        {
            throw new NotImplementedException();
        }
    }
}
