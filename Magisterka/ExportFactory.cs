using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HVACSimulator
{
    public enum EFileFormat
    {
        xls,
        csv
    }

    public class ExportFactory
    {
        public ExportFactory()
        {
            testData.PointsList.Add(new OxyPlot.DataPoint(1, 1));
            testData.PointsList.Add(new OxyPlot.DataPoint(2, 2));

        }

        public PlotData testData = new PlotData(EDataType.temperature, "Xopis", "Yopis", "title");

        public IExportsPlotData GetExportObject(EFileFormat fileFormat)
        {
            switch(fileFormat)
            {
                case EFileFormat.csv:
                    return new ExportManagerCSV();
                case EFileFormat.xls:
                    return new ExportManagerXLS();
                default:
                    throw new NotSupportedException("Brak możliwości eksportu do pliku z takim rozszerzeniem");
            }
        }

        public IExportsPlotData GetExportObject(EFileFormat fileFormat, string name)
        {
            switch (fileFormat)
            {
                case EFileFormat.csv:
                    return new ExportManagerCSV(name);
                case EFileFormat.xls:
                    return new ExportManagerXLS(name);
                default:
                    throw new NotSupportedException("Brak możliwości eksportu do pliku z takim rozszerzeniem");
            }
        }

    }
}
