using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public interface IExportsPlotData
    {
        void ExportPlotData(PlotData plotData);

        void ExportPlotDataRange(List<PlotData> plotDataList);
    }
}
