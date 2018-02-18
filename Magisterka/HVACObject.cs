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
        public Visibility ImageVisibility { get; set; }
        private bool _IsPresent;
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
        public Image ImageToDraw { get; set; }

        
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
    }
}
