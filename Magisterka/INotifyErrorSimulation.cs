using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    interface INotifyErrorSimulation
    {
        event EventHandler<string> SimulationErrorOccured;

        void OnSimulationErrorOccured(string error);

        void GetSubscription();
        
    }
}
