using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            SubscribeToAllItems();
        }

        public void UpdateParams()
        {
            throw new NotImplementedException();
        }

        private void SubscribeToAllItems()
        {
            foreach(HVACObject obj in HVACObjectsList)
            {
                obj.PropertyChanged += PresenceChanged;
            }
        }

        public event EventHandler<EventArgs> SupplyPresenceChanged;

        private void OnSupplyPresenceChanged()
        {
            SupplyPresenceChanged?.Invoke(this, EventArgs.Empty);
        }

        private void PresenceChanged(object sender, PropertyChangedEventArgs e)
        {
            OnSupplyPresenceChanged();
        }
    }
}
