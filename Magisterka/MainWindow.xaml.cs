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

namespace Magisterka
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private cRegulator regulator;
        private DispatcherTimer MainTimer;

        public MainWindow()
        {
            regulator = new cRegulator();
            MainTimer = new DispatcherTimer();
            MainTimer.Tick += MainTimer_Tick;
            InitializeComponent();
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            return;
        }

        private void SetTimerInterval(int seconds)
        {
            this.MainTimer.Interval = new TimeSpan(0, 0, seconds);
        }




        private void ControllerParametersButton_Click(object sender, RoutedEventArgs e)
        {
            regulator.ZmienParametry();
        }
    }
}
