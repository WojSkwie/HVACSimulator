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

namespace Magisterka
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private cRegulator regulator;
        private DispatcherTimer mainTimer;
        private HVACSupplyChannel supplyChannel;
        private List<Image> imagesSupplyChannnel;

        public MainWindow()
        {
            regulator = new cRegulator();
            mainTimer = new DispatcherTimer();
            supplyChannel = new HVACSupplyChannel();
            imagesSupplyChannnel = new List<Image>();

            InitializeComponent();

            mainTimer.Tick += MainTimer_Tick;
            supplyChannel.ChannelPresenceChanged += PresenceChangedInSupplyChannel;
            

            DataContext = supplyChannel;
            AddImagesToLists();
            DrawSupplyItems();
            
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            UpdateAllDynamicObjects();
            PressureDropSupplyNumeric.Value = supplyChannel.FanPressureDrop;
            FlowRateSupplyNumeric.Value = supplyChannel.FlowRate;
        }

        private void UpdateAllDynamicObjects()
        {
            supplyChannel.UpdateParams();
            foreach(HVACObject obj in supplyChannel.HVACObjectsList)
            {
                if(obj is HVACFan)
                {
                    ActualSpeedSupplyNumeric.Value = ((HVACFan)obj).ActualSpeedPercent;
                }
            }
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
            mainTimer.Start();
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            
            mainTimer.Stop();
        }

        private void ResetSimulation()
        {
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
            DrawSupplyItems();
        }

        private void AddImagesToLists()
        {
            imagesSupplyChannnel.Add(imgin1);
            imagesSupplyChannnel.Add(imgin2);
            imagesSupplyChannnel.Add(imgin3);
            imagesSupplyChannnel.Add(imgin4);
            imagesSupplyChannnel.Add(imgin5);
        }

        private void DrawSupplyItems()
        {
            for(int i = 0; i < supplyChannel.HVACObjectsList.Count; i++)
            {
                HVACObject obj = supplyChannel.HVACObjectsList[i];
                imagesSupplyChannnel[i].Source = new BitmapImage(new Uri(obj.ImageSource, UriKind.Relative)); 
            }
        }
        

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            int index = supplyDataGrid.SelectedIndex;
            if (index < 0) return;
            supplyChannel.MoveObject(index, -1);
            supplyDataGrid.ItemsSource = supplyChannel.HVACObjectsList;
            DrawSupplyItems();
            
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            int index = supplyDataGrid.SelectedIndex;
            if (index < 0) return;
            supplyChannel.MoveObject(index, 1);
            supplyDataGrid.ItemsSource = supplyChannel.HVACObjectsList;
            DrawSupplyItems();

        }

        private void EditCharItem_Click(object sender, RoutedEventArgs e)
        {
            int index = supplyDataGrid.SelectedIndex;
            if (index < 0) { return; }
            var currentItem = supplyChannel.HVACObjectsList[index];
            if (currentItem is IModifiableCharact)
            {
                (currentItem as IModifiableCharact).ModifyCharacteristics();
            }
        }

        private void SetSpeedSupplyNumeric_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if(SetSpeedSupplyNumeric.Value != null)
            {
                supplyChannel.SetSpeedFan((double)SetSpeedSupplyNumeric.Value);
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
            supplyChannel.ModifyCharacteristics();
        }
    }
}
