using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ahuKlasy;
using ahuRegulator;
using System.Windows.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using MahApps.Metro.Controls.Dialogs;

namespace HVACSimulator
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private cRegulator regulator;
        private DispatcherTimer mainTimer;
        private ExchangerViewModel ExchangerViewModel;
        private SeriesViewModel SeriesViewModel;
        private GlobalParameters GlobalParameters;
        private ExportFactory ExportFactory;

        public MainWindow()
        {
            GlobalParameters = GlobalParameters.Instance;
            regulator = new cRegulator();
            mainTimer = new DispatcherTimer();
            ExchangerViewModel = new ExchangerViewModel();
            SeriesViewModel = new SeriesViewModel();
            ExportFactory = new ExportFactory();

            InitializeComponent();

            mainTimer.Tick += MainTimer_Tick;
            ExchangerViewModel.supplyChannel.ChannelPresenceChanged += PresenceChangedInSupplyChannel;
            ExchangerViewModel.exhaustChannel.ChannelPresenceChanged += PresenceChangedInExchaustChannel;
            AddImagesSupply();
            AddImagesExhaust();
            DrawItemsForChannel(ExchangerViewModel.supplyChannel);
            DrawItemsForChannel(ExchangerViewModel.exhaustChannel);

            DataContext = ExchangerViewModel;
            supplyDataGrid.DataContext = ExchangerViewModel.supplyChannel;
            extractDataGrid.DataContext = ExchangerViewModel.exhaustChannel;
            Plot.DataContext = SeriesViewModel;
            PlotSeries.DataContext = SeriesViewModel;
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            UpdateAllDynamicObjects();
            ActualSpeedSupplyNumeric.Value = ExchangerViewModel.GetSpeedFromSupplyChannel();
            ActualHotWaterTemperatureNumeric.Value = ExchangerViewModel.GetHotWaterTempeartureFromSuppyChannel();
            ActualColdWaterTemperatureNumeric.Value = ExchangerViewModel.GetColdWaterTemperatureFromSupplyChannel();
            PressureDropSupplyNumeric.Value = ExchangerViewModel.GetPressureDropFromSupplyChannel();
            FlowRateSupplyNumeric.Value = ExchangerViewModel.GetFlowRateFromSupplyChannel();
            ExchangerViewModel.supplyChannel.CalculateAirParameters();

            GlobalParameters.IncrementTime();
            PlotSeries.ItemsSource = SeriesViewModel.ActualPoints;
            Plot.InvalidatePlot(true);

            TEMP.Value = ExchangerViewModel.supplyChannel.TEMP;
            TEMP2.Value = ExchangerViewModel.supplyChannel.TEMP2;
        }

        private void UpdateAllDynamicObjects()
        {
            ExchangerViewModel.UpdateParams();
        }

        private void ChangeTimerSpan(int milliseconds)
        {
            if(mainTimer != null)
            {
                bool wasEnabled = mainTimer.IsEnabled;
                mainTimer.Stop();
                mainTimer.Interval = new TimeSpan(0, 0, 0, 0, milliseconds);
                if(wasEnabled) mainTimer.Start();
            }
        }

        private void ControllerParametersButton_Click(object sender, RoutedEventArgs e)
        {
            regulator.ZmienParametry();
        }

        private void StepspersecSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int steps = (int)(Math.Pow(2, (int)e.NewValue-1));
            int milliseconds = 1000/steps;
            stepspersecLabel.Content = $"{steps} kroków na sek";
            ChangeTimerSpan(milliseconds);
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //supplyDataGrid.ItemsSource = supplyChannel.HVACObjectsList;
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            mainTimer.Stop();
            ResetSimulation();
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalParameters.SimulationState == EState.running) return;
            if (GlobalParameters.SimulationState == EState.stopped) 
            {
                SeriesViewModel.InitializeModelFromList(ExchangerViewModel.supplyChannel.HVACObjectsList);
                PresentObjectsSplitButton.DataContext = SeriesViewModel;
                Plot.DataContext = SeriesViewModel;
            }
            mainTimer.Start();
            GlobalParameters.SimulationState = EState.running;

        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalParameters.SimulationState = EState.paused;
            mainTimer.Stop();
        }

        private void ResetSimulation()
        {
            if (GlobalParameters.SimulationState == EState.stopped) return;
            throw new NotImplementedException();
        }

        private void steplengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int index = (int)e.NewValue - 1;
            steplengthLabel.Content = $"krok długości \n{Constants.stepValues[index]} sek";
            Constants.step = Constants.stepValues[index];
        }

        private void PresenceChangedInSupplyChannel(object sender, EventArgs e)
        {
            DrawItemsForChannel(ExchangerViewModel.supplyChannel);
        }

        private void PresenceChangedInExchaustChannel(object sender, EventArgs e)
        {
            DrawItemsForChannel(ExchangerViewModel.exhaustChannel);
        }


        private void AddImagesSupply()
        {
            ExchangerViewModel.imagesSupplyChannnel.Add(imgin1);
            ExchangerViewModel.imagesSupplyChannnel.Add(new Image());
            ExchangerViewModel.imagesSupplyChannnel.Add(imgin2);
            ExchangerViewModel.imagesSupplyChannnel.Add(imgin3);
            ExchangerViewModel.imagesSupplyChannnel.Add(imgin4);
            ExchangerViewModel.imagesSupplyChannnel.Add(imgin5);
        }

        private void AddImagesExhaust()
        {
            ExchangerViewModel.imagesExhaustChannel.Add(imgout1);
            ExchangerViewModel.imagesExhaustChannel.Add(new Image());
        }

        /*private void DrawSupplyItems()
        {
            for(int i = 0; i < ExchangerViewModel.supplyChannel.HVACObjectsList.Count; i++)
            {
                HVACObject obj = ExchangerViewModel.supplyChannel.HVACObjectsList[i];
                ExchangerViewModel.imagesSupplyChannnel[i].Source = new BitmapImage(new Uri(obj.ImageSource, UriKind.Relative));
                ExchangerViewModel.imagesSupplyChannnel[i].Visibility = ExchangerViewModel.supplyChannel.HVACObjectsList[i].IsPresent ? Visibility.Visible : Visibility.Hidden;
            }
        }*/

        private void DrawItemsForChannel(AirChannel airChannel)
        {
            for (int i = 0; i < airChannel.HVACObjectsList.Count; i++)
            {
                HVACObject obj = airChannel.HVACObjectsList[i];
                airChannel.ImagesList[i].Source = new BitmapImage(new Uri(obj.ImageSource, UriKind.Relative));
                airChannel.ImagesList[i].Visibility = airChannel.HVACObjectsList[i].IsPresent ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private void EditCharItem_Click(object sender, RoutedEventArgs e)
        {
            int index = supplyDataGrid.SelectedIndex;
            if (index < 0) { return; }
            var currentItem = ExchangerViewModel.supplyChannel.HVACObjectsList[index];
            if (currentItem is IModifiableCharact)
            {
                (currentItem as IModifiableCharact).ModifyCharacteristics();
            }
        }

        private void SetSpeedSupplyNumeric_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if(SetSpeedSupplyNumeric.Value != null)
            {
                ExchangerViewModel.supplyChannel.SetSpeedFan((double)SetSpeedSupplyNumeric.Value);
            }
        }

        private void ActualSpeedSupplyNumeric_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            double value = (double)ActualSpeedSupplyNumeric.Value;
            if (value.ToString().Length > 5)
            {
                string truncated = value.ToString("0.000");
                ActualSpeedSupplyNumeric.Value = double.Parse(truncated);
            }
        }

        private void EditCharChan_Click(object sender, RoutedEventArgs e)
        {
            ExchangerViewModel.supplyChannel.ModifyCharacteristics();
        }

        private void MoveButton_Click(object sender, RoutedEventArgs e)
        {
            int index = supplyDataGrid.SelectedIndex;
            int direction = Convert.ToInt32(((Button)sender).Tag);
            if (index < 0) return;
            ExchangerViewModel.supplyChannel.MoveObject(index, direction);
            //supplyDataGrid.ItemsSource = ExchangerViewModel.supplyChannel.HVACObjectsList; //TODO jeżeli przesuwanie przestanie działać to tutaj może być przyczyna
            DrawItemsForChannel(ExchangerViewModel.supplyChannel);
            //DrawItemsForChannel(ExchangerViewModel.exhaustChannel);
            //DrawSupplyItems();
        }

        private void SetHotWaterTemperatureNumeric_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (SetHotWaterTemperatureNumeric.Value != null)
            {
                ExchangerViewModel.supplyChannel.SetHeaterWaterTemperature((double)SetHotWaterTemperatureNumeric.Value);
            }
        }

        private void ActualHotWaterTemperatureNumeric_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            double value = (double)ActualHotWaterTemperatureNumeric.Value;
            if (value.ToString().Length > 5)
            {
                string truncated = value.ToString("0.000");
                ActualHotWaterTemperatureNumeric.Value = double.Parse(truncated);
            }
        }

        private void HotWaterFlowPercentNumeric_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (HotWaterFlowPercentNumeric.Value != null)
            {
                ExchangerViewModel.supplyChannel.SetHotWaterFlow((double)HotWaterFlowPercentNumeric.Value);
            }
        }

        private void SetColdWaterTemperatureNumeric_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if(SetColdWaterTemperatureNumeric.Value != null)
            {
                ExchangerViewModel.supplyChannel.SetColdWaterFlow((double)SetColdWaterTemperatureNumeric.Value);
            }
        }

        private void ActualColdWaterTemperatureNumeric_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            double value = (double)ActualColdWaterTemperatureNumeric.Value;
            if (value.ToString().Length > 5)
            {
                string truncated = value.ToString("0.000");
                ActualColdWaterTemperatureNumeric.Value = double.Parse(truncated);
            }
        }

        private void ColdWaterFlowPercentNumeric_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (ColdWaterFlowPercentNumeric.Value != null)
            {
                ExchangerViewModel.supplyChannel.SetColdWaterFlow((double)ColdWaterFlowPercentNumeric.Value);
            }
        }

        private void PresentObjectsSplitButton_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PlotDataSplitButton.ItemsSource = null; 
            if (((SplitButton)sender).SelectedItem == null) return;
            HVACObject obj = (HVACObject)((SplitButton)sender).SelectedItem;
            if (obj == null) return;
            PlotDataSplitButton.ItemsSource = obj.PlotDataList;
        }

        private void PlotDataSplitButton_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SeriesViewModel.ResetModel();
            if (PlotDataSplitButton.SelectedItem == null) return;
            SeriesViewModel.SetObjectToDrawPlot((HVACObject)PresentObjectsSplitButton.SelectedItem, ((PlotData)PlotDataSplitButton.SelectedItem).DataType);
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if(FileFormatSplitButton.SelectedItem == null) { this.ShowMessageAsync("Błąd", "Wybierz z listy rozszerzenie pliku"); return; }
            PlotData dataToExport = PlotDataSplitButton.SelectedItem as PlotData;
            if (dataToExport == null) { this.ShowMessageAsync("Błąd", "Wybierz obiekt i parametr do eksportowania"); return; }
            if (dataToExport.PointsList.Count == 0) { this.ShowMessageAsync("Błąd", "Brak danych do wyeksportowania"); return; }

            EFileFormat fileFormat = (EFileFormat)FileFormatSplitButton.SelectedItem;

            IExportsPlotData exporter = ExportFactory.GetExportObject(fileFormat);
            exporter.ExportPlotData(dataToExport);
        }

        private void ExportAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (FileFormatSplitButton.SelectedItem == null) { this.ShowMessageAsync("Błąd", "Wybierz z listy rozszerzenie pliku"); return; }
            EFileFormat fileFormat = (EFileFormat)FileFormatSplitButton.SelectedItem;

            IExportsPlotData exporter = ExportFactory.GetExportObject(fileFormat);

            List<PlotData> plotDataList = new List<PlotData>();

            foreach(PlotableObject obj in SeriesViewModel.PresentObjects)
            {
                plotDataList.AddRange(obj.GetAllPlotData());
            }
            if (plotDataList.Count == 0) { this.ShowMessageAsync("Błąd", "Brak danych do wyeksportowania"); return; }
            exporter.ExportPlotDataRange(plotDataList);
        }

        
    }
}
