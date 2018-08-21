using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HVACSimulator
{
    public abstract class HVACObject : PlottableObject, INotifyPropertyChanged, IModifiableCharact, INotifyErrorSimulation//, IReturnsPlotData
    {
        public bool IsMutable { get; protected set; } 
        public bool IsGenerativeFlow { get; set; }
        public double ACoeff { get; set; }
        public double BCoeff { get; set; }
        public double CCoeff { get; set; }
        protected bool _IsPresent;
        private GlobalParameters GlobalParameters;
        public virtual bool IsPresent
        {
            get { return _IsPresent; }
            set
            {
                this._IsPresent = value;
                OnPropertyChanged("IsPresent");
            }
        }
        public string Name { get; set; }
        public bool IsMovable { get; protected set; }
        public string ImageSource { get; set; }
        public bool HasSingleTimeConstant { get; set; }
        public double TimeConstant { get; set; }
        public Air OutputAir { get; set; }
        //public List<PlotData> PlotDataList { get; set; }

        public HVACObject() : base()
        {
            GlobalParameters = GlobalParameters.Instance;
            //PlotDataList = new List<PlotData>();
            IsMutable = true;
            _IsPresent = true;
            InitializePlotDataList();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<string> SimulationErrorOccured;

        protected void OnPropertyChanged(string propertyName)
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
            OutputAir = (Air)inputAir.Clone();
            AddDataPointFromAir(OutputAir, EDataType.humidity);
            AddDataPointFromAir(OutputAir, EDataType.temperature);
            return OutputAir;

        }

        protected override void InitializePlotDataList()
        {
            PlotData tempPlotData = new PlotData(EDataType.temperature, "Czas [s]", "Temperatura *C", Name); //TODO stopnie
            PlotDataList.Add(tempPlotData);
            PlotData humidPlotData = new PlotData(EDataType.humidity, "Czas [s]", "Wilgotność [%RH]", Name);
            PlotDataList.Add(humidPlotData);
        }

        public void AddDataPointFromAir(Air air, EDataType dataType)
        {
            PlotData plotData = PlotDataList.Single(item => item.DataType == dataType);
            DataPoint newPoint;
            switch (dataType)
            {
                case EDataType.temperature:
                    newPoint = new DataPoint(GlobalParameters.SimulationTime, air.Temperature);
                    break;
                case EDataType.humidity:
                    newPoint = new DataPoint(GlobalParameters.SimulationTime, air.RelativeHumidity);
                    break;
                default:
                    OnSimulationErrorOccured("Niewłaściwy rodzaj pobieranych danych z obiektu " + this.ToString());
                    return;
            }
            plotData.AddPointWithEvent(newPoint);
            //plotData.PointsList.Add(newPoint);
            //OnNewPointCreated(newPoint);
        }

        protected void SetPlotDataNames()
        {
            foreach(PlotData plotData in PlotDataList)
            {
                plotData.PlotTitle = Name;
            }
        }
    }
}
