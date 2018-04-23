using OxyPlot;
using System;
using System.Collections.Generic;
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
        humidity

    }

    public class PlotData
    {
        public List<DataPoint> PointsList { get; set; }
        public EDataType DataType { get; set; }

        public string XAxisTitle { get; set; }
        public string YAxisTitle { get; set; }
        public string PlotTile { get; set; }

        public PlotData()
        {

        }

        public PlotData(EDataType dataType, string xAxisTitle, string yAxisTitle, string plotTile)
        {
            PointsList = new List<DataPoint>();
            DataType = dataType;
            XAxisTitle = xAxisTitle;
            YAxisTitle = yAxisTitle;
            PlotTile = plotTile;
        }
    }
}
