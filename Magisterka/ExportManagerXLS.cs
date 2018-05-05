using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.Model; 
using NPOI.HSSF.UserModel;
using System.IO;

namespace HVACSimulator
{
    public class ExportManagerXLS : IExportsPlotData
    {
        private HSSFWorkbook wb = HSSFWorkbook.Create(InternalWorkbook.CreateWorkbook());
        private HSSFSheet sh;
        private string name;


        public ExportManagerXLS(string path)
        {
            sh = (HSSFSheet)wb.CreateSheet("Sheet1");
            name = path + ".xls";
            int counter = 1;
            while (File.Exists(name))
            {
                name = path + counter + ".xls";
                counter++;
            }
        }

        public ExportManagerXLS()
        {
            sh = (HSSFSheet)wb.CreateSheet("Sheet1");
            name = "Export_" + DateTime.Now.ToString().Replace(":", "-") + ".xls";
        }

        public void ExportPlotData(PlotData plotData)
        {
            int counter = 0;
            var row = sh.CreateRow(counter++);
            var cell = row.CreateCell(0);
            cell.SetCellValue(plotData.PlotTitle);
            row = sh.CreateRow(counter++);
            cell = row.CreateCell(0);
            cell.SetCellValue(plotData.XAxisTitle);
            cell = row.CreateCell(1);
            cell.SetCellValue(plotData.YAxisTitle);
            
            for (int i = 0; i < plotData.PointsList.Count; i++)
            {
                var r = sh.CreateRow(i + counter);
                var c = r.CreateCell(0);
                c.SetCellValue(plotData.PointsList[i].X);
                c = r.CreateCell(1);
                c.SetCellValue(plotData.PointsList[i].Y);
            }
            SaveWorkBook(wb);
        }

        public void ExportPlotDataRange(List<PlotData> plotDataList)
        {
            int maxCount = plotDataList.Max(item => item.PointsList.Count);
            int maxIndex = plotDataList.Select(item => item.PointsList.Count).MaxIndex();

            int counter = 0;
            var row = sh.CreateRow(counter++);
            var cell = row.CreateCell(0);
            cell.SetCellValue("Eksport zbiorczy danych");
            row = sh.CreateRow(counter++);
            cell = row.CreateCell(0);
            cell.SetCellValue("Lista obiektów ->");
            for (int i = 1; i <= plotDataList.Count; i++) 
            {
                cell = row.CreateCell(i);
                cell.SetCellValue(plotDataList[i-1].PlotTitle);
            }

            row = sh.CreateRow(counter++);
            cell = row.CreateCell(0);
            cell.SetCellValue(plotDataList[maxIndex].XAxisTitle);
            for (int i = 1; i <= plotDataList.Count; i++)
            {
                cell = row.CreateCell(i);
                cell.SetCellValue(plotDataList[i - 1].YAxisTitle);
            }
            
            for (int i = 0; i < maxCount; i++)
            {
                row = sh.CreateRow(counter++);
                cell = row.CreateCell(0);
                cell.SetCellValue(plotDataList[maxIndex].PointsList[i].X);
                for (int j = 0; j < plotDataList.Count; j++) 
                {
                    cell = row.CreateCell(j + 1);
                    if (i < plotDataList[j].PointsList.Count) cell.SetCellValue(plotDataList[j].PointsList[i].Y);
                    else cell.SetCellValue(" ");
                }
            }

            SaveWorkBook(wb);
        }

        private void SaveWorkBook(HSSFWorkbook wb)
        {
            using (var fs = new FileStream(name, FileMode.Create, FileAccess.Write))
            {
                wb.Write(fs);
            }
        }
    }
}
