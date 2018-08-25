using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACSimulator
{
    public class HVACEnvironment :PlottableObject
    {
        public Air Air;
        public string Name { get; set; } = "Środowisko zewnętrzne";

        public HVACEnvironment()
        {
            InitializePlotDataList();
            Air = new Air(-5, 40, EAirHum.relative);
        }

        protected override void InitializePlotDataList()
        {
            PlotData tempPlotData = new PlotData(EDataType.temperature, "Czas [s]", "Temperatura *C", "Temperatura zewnętrzna"); //TODO stopnie
            PlotDataList.Add(tempPlotData);
            PlotData humidPlotData = new PlotData(EDataType.humidity, "Czas [s]", "Wilgotność [%RH]", "Wilgotność zewnętrzna");
            PlotDataList.Add(humidPlotData);
        }
    }
}
