using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    interface IReturnsPlotData
    {
        PlotData GetPlotData(EDataType dataType);

        /*event EventHandler<DataPoint> NewPointCreated;
        void OnNewPointCreated(DataPoint dataPoint);*/
        
    }
}
