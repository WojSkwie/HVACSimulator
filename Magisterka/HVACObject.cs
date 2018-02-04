using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka
{
    public abstract class HVACObject : INotifyPropertyChanged
    {
        public bool IsGenerativeFlow { get; set; }
        public double ACoeff { get; set; }
        public double BCoeff { get; set; }
        public double CCoeff { get; set; }
        private bool _IsPresent;
        public bool IsPresent
        {
            get { return _IsPresent; }
            set
            {
                OnPropoertyChanged("IsPresent");
                this._IsPresent = value;
            }
        }
        public string Name { get; set; }

        
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropoertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
