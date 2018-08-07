using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public abstract class BindableAnalogPort : INotifyErrorSimulation
    {
        public double MaxValue { get; set; }
        public double MinValue { get; set; }
        protected int max12BitsNumber = 4095;

        public event EventHandler<string> SimulationErrorOccured;

        public BindableAnalogPort(double max, double min)
        {
            MaxValue = max;
            MinValue = min;
        }

        public void OnSimulationErrorOccured(string error)
        {
            throw new NotImplementedException();
        }

        public bool ValidateValue(int parameter)
        {
            if (parameter > max12BitsNumber || parameter < 0)
            {
                OnSimulationErrorOccured(string.Format("Nieprawidłowa wartość parametru: {0}", parameter));
                return false;
            }
            return true;
        }
    }
}
