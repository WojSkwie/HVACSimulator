using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public abstract class PlottableObject : IResetableObject
    {
        public List<PlotData> PlotDataList { get; set; }

        public PlottableObject()
        {
            PlotDataList = new List<PlotData>();
        }

        public PlotData GetPlotData(EDataType dataType)
        {
            if (!PlotDataList.Any(item => item.DataType == dataType)) return null;
            PlotData plotData = PlotDataList.First(item => item.DataType == dataType);
            return plotData;
        }

        public List<PlotData> GetAllPlotData()
        {
            return PlotDataList;
        }

        protected abstract void InitializePlotDataList();

        public virtual void SetInitialValuesParameters()
        {
            foreach(var plotData in PlotDataList)
            {
                plotData.PointsList.Clear();
            }
            //throw new NotImplementedException();
        }
    }
}
