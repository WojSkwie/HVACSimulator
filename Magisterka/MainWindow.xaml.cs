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
        private HVACSupplyChannel supplyChannel = new HVACSupplyChannel();
        public ObservableCollection<HVACObject> HVACObjectsList111 { get; set; } = new ObservableCollection<HVACObject>();

        public MainWindow()
        {
            HVACObjectsList111.Add(new HVACFilter());
            HVACObjectsList111.Add(new HVACFilter());
            HVACObjectsList111.Add(new HVACFilter());
            HVACObjectsList111.Add(new HVACFilter());
            regulator = new cRegulator();
            mainTimer = new DispatcherTimer();

            InitializeComponent();

            mainTimer.Tick += MainTimer_Tick;
            supplyChannel.ChannelPresenceChanged += PresenceChangedInSupplyChannel;

            
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
            //return;
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

        private void stepspersecSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int steps = (int)(Math.Pow(2, (int)e.NewValue-1));
            int milliseconds = 1000/steps;
            stepspersecLabel.Content = $"{steps} kroków na sek";
            ChangeTimerSpan(milliseconds);
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
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
        }

        private void PresenceChangedInSupplyChannel(object sender, EventArgs e)
        {
            DrawSupplyItems();
        }

        private void DrawSupplyItems()
        {

        }
        

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            int index = supplyDataGrid.SelectedIndex;
            if (index > 0)
            {
                if(supplyChannel.HVACObjectsList[index].IsMovable && supplyChannel.HVACObjectsList[index - 1].IsMovable)
                {
                    HVACObject temp = supplyChannel.HVACObjectsList[index - 1];
                    supplyChannel.HVACObjectsList.RemoveAt(index - 1);
                    supplyChannel.HVACObjectsList.Insert(index,temp);
                    supplyDataGrid.ItemsSource = supplyChannel.HVACObjectsList;
                }
            }
            
            
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            int index = supplyDataGrid.SelectedIndex;
            if (index < 0) return;
            if (index < supplyChannel.HVACObjectsList.Count-1 && index !=0)
            {
                if (supplyChannel.HVACObjectsList[index].IsMovable && supplyChannel.HVACObjectsList[index + 1].IsMovable)
                {
                    HVACObject temp = supplyChannel.HVACObjectsList[index + 1];
                    supplyChannel.HVACObjectsList.RemoveAt(index + 1);
                    supplyChannel.HVACObjectsList.Insert(index, temp);
                    supplyDataGrid.ItemsSource = supplyChannel.HVACObjectsList;
                }
            }
        }

        /*private int FindRowIndex(object sender)
        {
            int index = -1;
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
            {
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    index = row.GetIndex();
                    
                }
            }
            return index;
        }*/

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
    }
}
