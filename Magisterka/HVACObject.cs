using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Magisterka
{
    public abstract class HVACObject : INotifyPropertyChanged, IModifiableCharact, INotifyErrorSimulation
    {
        public bool IsGenerativeFlow { get; set; }
        public double ACoeff { get; set; }
        public double BCoeff { get; set; }
        public double CCoeff { get; set; }
        private bool _IsPresent = true;
        public bool IsPresent
        {
            get { return _IsPresent; }
            set
            {
                this._IsPresent = value;
                OnPropertyChanged("IsPresent");
            }
        }
        public string Name { get; set; }
        public bool IsMovable { get; set; }
        public string ImageSource { get; set; }
        public bool HasSingleTimeConstant { get; set; }
        public double TimeConstant { get; set; }
        //public double HeatTransferCoeff { get; set; }
        //public double HeatExchangeSurface { get; set; }
        public Air OutputAir { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<string> SimulationErrorOccured;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ModifyCharacteristics()
        {
            var CharactDialog = new CharactWindow(this);
            CharactDialog.ShowDialog();
        }

        public void OnSimulationErrorOccured(string error)
        {
            SimulationErrorOccured?.Invoke(this, error);
            MessageBox.Show(error);
        }

        public virtual Air CalculateOutputAirParameters(Air inputAir, double airFlow)
        {
            return (OutputAir = (Air)inputAir.Clone());
        }
    }
}
