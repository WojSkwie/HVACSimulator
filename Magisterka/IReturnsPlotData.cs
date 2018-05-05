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

        List<PlotData> GetAllPlotData();

        //void InitializePlotDataList();
        
        
    }
}
