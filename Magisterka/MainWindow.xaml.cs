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
        private AdapterViewModel AdapterViewModel;

        public MainWindow()
        {
            GlobalParameters = GlobalParameters.Instance;
            regulator = new cRegulator();
            mainTimer = new DispatcherTimer();
            ExchangerViewModel = new ExchangerViewModel();
            SeriesViewModel = new SeriesViewModel();
            ExportFactory = new ExportFactory();
            AdapterViewModel = new AdapterViewModel(ExchangerViewModel);
            
            InitializeComponent();

            AdapterTabItem.DataContext = AdapterViewModel;
            mainTimer.Tick += MainTimer_Tick;
            ExchangerViewModel.SupplyChannel.ChannelPresenceChanged += PresenceChangedInSupplyChannel;
            ExchangerViewModel.ExhaustChannel.ChannelPresenceChanged += PresenceChangedInExchaustChannel;
            AddImagesSupply();
            AddImagesExhaust();
            DrawItemsForChannel(ExchangerViewModel.SupplyChannel);
            DrawItemsForChannel(ExchangerViewModel.ExhaustChannel);

            DataContext = ExchangerViewModel;
            supplyDataGrid.DataContext = ExchangerViewModel.SupplyChannel;
            extractDataGrid.DataContext = ExchangerViewModel.ExhaustChannel;
            Plot.DataContext = SeriesViewModel;
            PlotSeries.DataContext = SeriesViewModel;
            ExchangerViewModel.InitializeDataContextsForControlNumerics(SetSpeedSupplyNumeric, ColdWaterFlowPercentNumeric, HotWaterFlowPercentNumeric, MixingPercentNumeric);
            PresentObjectsSplitButton.DataContext = SeriesViewModel;
            AirTempeartureSlider.DataContext = ExchangerViewModel.Environment.Air;
            AirHumiditySlider.DataContext = ExchangerViewModel.Environment.Air;
            AirTemperatureNumeric.DataContext = ExchangerViewModel.Environment.Air;
            AirHumidityNumeric.DataContext = ExchangerViewModel.Environment.Air;

            GlobalParameters.SimulationErrorOccured += OnErrorInSimulationOccured;

        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            AdapterViewModel.SendValuesToAdapter();
            Air outsideAir = ExchangerViewModel.Environment.Air;
            Air roomAir = ExchangerViewModel.Room.AirInRoom;
            ExchangerViewModel.CalculateAirFlowInChannels(out double airFlow, out double massFlow, out double pressureDrop, outsideAir);
            UpdateAllDynamicObjects();
            ActualSpeedSupplyNumeric.Value = ExchangerViewModel.GetSpeedFromSupplyChannel();
            ActualHotWaterTemperatureNumeric.Value = ExchangerViewModel.GetHotWaterTempeartureFromSuppyChannel();
            ActualColdWaterTemperatureNumeric.Value = ExchangerViewModel.GetColdWaterTemperatureFromSupplyChannel();
            PressureDropSupplyNumeric.Value = ExchangerViewModel.GetPressureDropFromSupplyChannel();
            FlowRateSupplyNumeric.Value = ExchangerViewModel.GetFlowRateFromSupplyChannel();

            Air exhaustExchangerAir = ExchangerViewModel.ExhaustChannel.CalculateAirParametersBeforeExchanger(roomAir, airFlow, massFlow);
            Air supplyExchangerAir = ExchangerViewModel.SupplyChannel.CalculateAirParametersBeforeExchanger(outsideAir, airFlow, massFlow);
            ExchangerViewModel.Exchanger.CalculateExchangeAndSetOutputAir(supplyExchangerAir, exhaustExchangerAir, out Air supplyAirAfter, out Air exhaustAirAfter);
            ExchangerViewModel.ExhaustChannel.CalculateAirParametersWithAndAfterExchanger(exhaustAirAfter, airFlow, massFlow);
            Air airEnteringRoom = ExchangerViewModel.SupplyChannel.CalculateAirParametersWithAndAfterExchanger(supplyAirAfter, airFlow, massFlow);
            ExchangerViewModel.Room.CalculateAirParametersInRoom(airEnteringRoom, airFlow, massFlow);
            ExchangerViewModel.Environment.AddDataPointsFromAir();

            GlobalParameters.IncrementTime();
            PlotSeries.ItemsSource = SeriesViewModel.ActualPoints;
            Plot.InvalidatePlot(true);
            AdapterViewModel.SendDataRequestToAdapter();
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
            if(AdapterViewModel.IsConnected)
            {
                if (GlobalParameters.SimulationState == EState.stopped)
                {
                    SeriesViewModel.InitializeModelFromList(ExchangerViewModel.SupplyChannel.HVACObjectsList);
                    SeriesViewModel.AddPlottableObject(ExchangerViewModel.SupplyChannel);
                    SeriesViewModel.AddPlottableObject(ExchangerViewModel.Environment);
                    SeriesViewModel.AddPlottableObject(ExchangerViewModel.Room);
                    Plot.DataContext = SeriesViewModel;
                }
                ExchangerViewModel.AllowChanges = false;
                mainTimer.Start();
                GlobalParameters.SimulationState = EState.running;
            }
            else
            {
                MessageBox.Show("Najpierw połącz się z adapterem");
            }
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalParameters.SimulationState = EState.paused;
            mainTimer.Stop();
        }

        private void ResetSimulation()
        {
            if (GlobalParameters.SimulationState == EState.stopped) return;
            GlobalParameters.SimulationState = EState.stopped;
            ExchangerViewModel.SetInitialValuesParameters();
            SeriesViewModel.SetInitialValuesParameters();
        }

        private void steplengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int index = (int)e.NewValue - 1;
            steplengthLabel.Content = $"krok długości \n{Constants.stepValues[index]} sek";
            Constants.step = Constants.stepValues[index];
        }

        private void PresenceChangedInSupplyChannel(object sender, EventArgs e)
        {
            DrawItemsForChannel(ExchangerViewModel.SupplyChannel);
        }

        private void PresenceChangedInExchaustChannel(object sender, EventArgs e)
        {
            DrawItemsForChannel(ExchangerViewModel.ExhaustChannel);
        }


        private void AddImagesSupply()
        {
            ExchangerViewModel.ImagesSupplyChannnel.Add(imgin1);
            ExchangerViewModel.ImagesSupplyChannnel.Add(new Image());
            ExchangerViewModel.ImagesSupplyChannnel.Add(imgin6);
            ExchangerViewModel.ImagesSupplyChannnel.Add(imgin2);
            ExchangerViewModel.ImagesSupplyChannnel.Add(imgin3);
            ExchangerViewModel.ImagesSupplyChannnel.Add(imgin4);
            ExchangerViewModel.ImagesSupplyChannnel.Add(imgin5);
        }

        private void AddImagesExhaust()
        {
            ExchangerViewModel.ImagesExhaustChannel.Add(imgout1);
            ExchangerViewModel.ImagesExhaustChannel.Add(imgout2);
            ExchangerViewModel.ImagesExhaustChannel.Add(new Image());
            ExchangerViewModel.ImagesExhaustChannel.Add(new Image());
        }

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
            var currentItem = supplyDataGrid.SelectedItem;
            if (currentItem == null) return;
            if (currentItem is IModifiableCharact)
            {
                (currentItem as IModifiableCharact).ModifyCharacteristics();
            }
        }

        private void EditCharExItem_Click(object sender, RoutedEventArgs e)
        {
            var currentItem = extractDataGrid.SelectedItem;
            if (currentItem == null) return;
            if (currentItem is IModifiableCharact)
            {
                (currentItem as IModifiableCharact).ModifyCharacteristics();
            }
        }

        private void EditCharChan_Click(object sender, RoutedEventArgs e)
        {
            ExchangerViewModel.SupplyChannel.ModifyCharacteristics();
        }

        private void EditCharExChan_Click(object sender, RoutedEventArgs e)
        {
            ExchangerViewModel.ExhaustChannel.ModifyCharacteristics();
        }

        private void MoveButton_Click(object sender, RoutedEventArgs e)
        {
            int index = supplyDataGrid.SelectedIndex;
            int direction = Convert.ToInt32(((Button)sender).Tag);
            if (index < 0) return;
            ExchangerViewModel.SupplyChannel.MoveObject(index, direction);
            //supplyDataGrid.ItemsSource = ExchangerViewModel.supplyChannel.HVACObjectsList; //TODO jeżeli przesuwanie przestanie działać to tutaj może być przyczyna
            DrawItemsForChannel(ExchangerViewModel.SupplyChannel);
            
        }

        private void SetHotWaterTemperatureNumeric_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (SetHotWaterTemperatureNumeric.Value != null)
            {
                ExchangerViewModel.SupplyChannel.SetHeaterWaterTemperature((double)SetHotWaterTemperatureNumeric.Value);
            }
        }

        private void HotWaterFlowPercentNumeric_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (HotWaterFlowPercentNumeric.Value != null)
            {
                ExchangerViewModel.SupplyChannel.SetHotWaterFlow((double)HotWaterFlowPercentNumeric.Value);
            }
        }

        private void SetColdWaterTemperatureNumeric_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if(SetColdWaterTemperatureNumeric.Value != null)
            {
                ExchangerViewModel.SupplyChannel.SetCoolerWaterTemperature((double)SetColdWaterTemperatureNumeric.Value);
            }
        }

        private void ColdWaterFlowPercentNumeric_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (ColdWaterFlowPercentNumeric.Value != null)
            {
                ExchangerViewModel.SupplyChannel.SetColdWaterFlow((double)ColdWaterFlowPercentNumeric.Value);
            }
        }

        private void PresentObjectsSplitButton_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PlotDataSplitButton.ItemsSource = null; 
            if (((SplitButton)sender).SelectedItem == null) return;
            PlottableObject obj = (PlottableObject)((SplitButton)sender).SelectedItem;
            if (obj == null) return;
            PlotDataSplitButton.ItemsSource = obj.PlotDataList;
        }

        private void PlotDataSplitButton_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SeriesViewModel.ResetModel();
            if (PlotDataSplitButton.SelectedItem == null) return;
            SeriesViewModel.SetObjectToDrawPlot((PlottableObject)PresentObjectsSplitButton.SelectedItem, ((PlotData)PlotDataSplitButton.SelectedItem).DataType);
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

            foreach(PlottableObject obj in SeriesViewModel.PresentObjects)
            {
                plotDataList.AddRange(obj.GetAllPlotData());
            }
            if (plotDataList.Count == 0) { this.ShowMessageAsync("Błąd", "Brak danych do wyeksportowania"); return; }
            exporter.ExportPlotDataRange(plotDataList);
        }

        private void ExchangerCharacteristicsButton_Click(object sender, RoutedEventArgs e)
        {
            ExchangerCharacteristicWindow exchangerCharacteristicWindow = new ExchangerCharacteristicWindow(ExchangerViewModel.Exchanger);
            exchangerCharacteristicWindow.ShowDialog();
        }

        private async void SearchAdapterButtonClick(object sender, RoutedEventArgs e)
        {
            AdapterViewModel.AllowClick = false;
            await Task.Delay(800);
            AdapterViewModel.SearchAdapter();
            AdapterViewModel.AllowClick = true;
        }

        private void ConnectToAdapterButtonClick(object sender, RoutedEventArgs e)
        {
            if(AdapterViewModel.IsConnected)
            {
                AdapterViewModel.Disconnect();
            }
            else
            {
                AdapterViewModel.Connect();
            }
        }

        private void OnErrorInSimulationOccured(object sender, string error)
        {
            mainTimer.Stop();
            ExchangerViewModel.AllowChanges = true;
            GlobalParameters.SimulationState = EState.paused;
        }

        private void RoomCharacteristicsButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
