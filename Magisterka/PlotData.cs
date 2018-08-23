using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{

    public enum EDataType
    {///koniecznie należy podać opis nowych elementów. w przeciwnym wypadku wysypie się aplikacja
        [Description("Temperatura")]
        temperature,

        [Description("Wilgotność")]
        humidity,

        [Description("Natężenie przepływu")]
        flowRate,

        [Description("Spadek ciśnienia")]
        pressureDrop,
    }

    public class PlotData : IResetableObject
    {
        public ObservableCollection<DataPoint> PointsList { get; set; }
        public EDataType DataType { get; set; }

        public string XAxisTitle { get; set; }
        public string YAxisTitle { get; set; }
        public string PlotTitle { get; set; }

        public event EventHandler<DataPoint> NewPointCreated;

        public PlotData(EDataType dataType, string xAxisTitle, string yAxisTitle, string plotTile)
        {
            PointsList = new ObservableCollection<DataPoint>();
            DataType = dataType;
            XAxisTitle = xAxisTitle;
            YAxisTitle = yAxisTitle;
            PlotTitle = plotTile;
        }

        public void AddPointWithEvent(DataPoint dataPoint)
        {
            PointsList.Add(dataPoint);
            OnNewPointCreated(dataPoint);
        }

        public void OnNewPointCreated(DataPoint dataPoint)
        {
            NewPointCreated?.Invoke(this, dataPoint);
        }

        public void SetInitialValuesParameters()
        {
            PointsList.Clear();
        }
    }
}
