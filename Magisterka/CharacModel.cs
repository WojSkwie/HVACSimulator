using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magisterka
{
    public class CharacModel : INotifyPropertyChanged
    {
        public CharacModel()
        {
            plotModel = new PlotModel();
            LineSeries = new LineSeries();
            SetUpLegend();
            SetUpAxes();
        }
        
        public LineSeries LineSeries { get; set; }
        private PlotModel plotModel;
        public PlotModel PlotModel
        {
            get
            {
                return plotModel;
            }
            set
            {
                plotModel = value; OnPropertyChanged("PlotModel");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void SetUpLegend()
        {
            PlotModel.LegendOrientation = LegendOrientation.Horizontal;
            PlotModel.LegendPlacement = LegendPlacement.Outside;
            PlotModel.LegendPosition = LegendPosition.TopRight;
            PlotModel.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            PlotModel.LegendBorder = OxyColors.Black; 
        }
        private void SetUpAxes()
        {
            PlotModel.Title = "Charakterystyka";
            var xAxis = new LinearAxis()
            {
                Position = AxisPosition.Bottom
            };
            PlotModel.Axes.Add(xAxis);
            var yAxis = new LinearAxis();
            PlotModel.Axes.Add(yAxis);
        }

        public void ClearPlot()
        {
            LineSeries.Points.Clear();
        }

        public void AddPoint(double X, double Y)
        {
            PlotModel.Series.Clear();
            LineSeries.Points.Add(new DataPoint(X, Y));
            PlotModel.Series.Add(LineSeries);
            plotModel.InvalidatePlot(true);
        }
    }
}
