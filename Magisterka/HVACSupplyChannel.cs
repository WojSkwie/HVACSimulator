using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka
{
    public sealed class HVACSupplyChannel : AirChannel, IDynamicObject
    {
        public HVACSupplyChannel() : base()
        {
            
            HVACObjectsList.Add(new HVACFilter());
            HVACObjectsList.Add(new HVACHeater());
            HVACObjectsList.Add(new HVACCooler());
            HVACObjectsList.Add(new HVACFan());
            HVACObjectsList.Add(new HVACFilter());
            SubscribeToAllItems();
        } 

        public void UpdateParams()
        {
            
            foreach(HVACObject obj in HVACObjectsList)
            {
                if(obj is IDynamicObject)
                {
                    ((IDynamicObject)obj).UpdateParams();
                }
            }
        }

        public void SetSpeedFan(double speed)
        {
            foreach(HVACObject obj in HVACObjectsList)
            {
                if(obj is HVACFan)
                {
                    ((HVACFan)obj).SetSpeedPercent = speed;
                }
            }
        }

        

        
    }
}
