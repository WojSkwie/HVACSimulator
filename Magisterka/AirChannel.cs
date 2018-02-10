using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka
{
    public abstract class AirChannel
    {
        public ObservableCollection<HVACObject> HVACObjectsList { get; set; } = new ObservableCollection<HVACObject>();

        protected AirChannel()
        {
        }

        protected void SubscribeToAllItems()
        {
            foreach (HVACObject obj in HVACObjectsList)
            {
                obj.PropertyChanged += PresenceChanged;
            }
        }

        public event EventHandler<EventArgs> ChannelPresenceChanged;

        protected void OnChannelPresenceChanged()
        {
            ChannelPresenceChanged?.Invoke(this, EventArgs.Empty);
        }

        protected void PresenceChanged(object sender, PropertyChangedEventArgs e)
        {
            OnChannelPresenceChanged();
        }
    }
}
