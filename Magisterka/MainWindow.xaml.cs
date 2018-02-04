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

        public MainWindow()
        {
            
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
            supplyDataGrid.ItemsSource = supplyChannel.HVACObjectsList;
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

        private void supplyDataGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            int itemIndex = supplyDataGrid.InputHitTest(e.GetPosition(this)).
        }
    }
}
