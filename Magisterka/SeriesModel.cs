using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;

namespace Magisterka
{
    class SeriesModel
    {
        public IList<DataPoint> Points { get; private set; }
        public string PlotTitle { get; set; }
        public string SeriesTitle { get; set; }

        public SeriesModel()
        {
            this.PlotTitle = "Wykres";
            this.SeriesTitle = "Seria 1";
            this.Points = new List<DataPoint>
                              {
                                  new DataPoint(0, 4),
                                  new DataPoint(10, 13),
                                  new DataPoint(20, 15),
                                  new DataPoint(30, 16),
                                  new DataPoint(40, 12),
                                  new DataPoint(50, 12)
                              };
        }

        public void addPoint(double x, double y)
        {
            this.Points.Add(new DataPoint(x, y));
        }

        public void clearPlot()
        {
            this.Points = null;
        }

        
    }
}
