using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace HVACSimulator
{
    public class ExportManagerCSV : IExportsPlotData
    {
        private CsvWriter csv;
        private Configuration configuration = new Configuration
        {
            Delimiter = ";"
        };

        public ExportManagerCSV()
        {
            csv = new CsvWriter(new StreamWriter("Export_" + DateTime.Now.ToString().Replace(":", "-") + ".csv"), configuration);
        }

        public ExportManagerCSV(string path)
        {
            string name = path + ".csv";
            int counter = 1;
            while (File.Exists(name)) 
            {
                name = path + counter + ".csv";
                counter++;
            }
            csv = new CsvWriter(new StreamWriter(name), configuration);

        }

        public void ExportPlotData(PlotData plotData)
        {
            csv.WriteField(plotData.PlotTitle);
            csv.NextRecord();
            csv.WriteField(plotData.XAxisTitle);
            csv.WriteField(plotData.YAxisTitle);
            csv.NextRecord();
            for (int i = 0; i < plotData.PointsList.Count; i++)
            {
                csv.WriteField(plotData.PointsList[i].X);
                csv.WriteField(plotData.PointsList[i].Y);
                csv.NextRecord();
            }
            csv.Dispose();
        }

        public void ExportPlotDataRange(List<PlotData> plotDataList)
        {
            int maxCount = plotDataList.Max(item => item.PointsList.Count);
            int maxIndex = plotDataList.Select(item => item.PointsList.Count).ToList().MaxIndex();
            csv.WriteField("Eksport zbiorczy danych");
            csv.NextRecord();
            csv.WriteField("Lista obiektów ->");
            foreach(PlotData plotData in plotDataList)
            {
                csv.WriteField(plotData.PlotTitle);
            }
            csv.NextRecord();

            csv.WriteField(plotDataList[maxIndex].XAxisTitle);
            foreach (PlotData plotData in plotDataList)
            {
                csv.WriteField(plotData.YAxisTitle);
            }
            csv.NextRecord();

            for (int i = 0; i < maxCount; i++)
            {
                csv.WriteField(plotDataList[maxIndex].PointsList[i].X);
                foreach(PlotData plotData in plotDataList)
                {
                    if (i < plotData.PointsList.Count) csv.WriteField(plotData.PointsList[i].Y);
                    else csv.WriteField(" ");
                }
                csv.NextRecord();
            }

            csv.Dispose();
        }
    }
}
