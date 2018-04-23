using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{

    public enum EDataType
    {
        temperature,
        humidity
    }

    public class PlotData
    {
        public List<DataPoint> PointsList { get; set; }
        public EDataType DataType { get; set; }

        public PlotData()
        {

        }
    }
}
