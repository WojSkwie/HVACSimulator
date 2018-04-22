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
using System.Windows.Shapes;

namespace HVACSimulator
{
    /// <summary>
    /// Interaction logic for ChannelCharactWindow.xaml
    /// </summary>
    public partial class ChannelCharactWindow : MetroWindow
    {
        private AirChannel AirChannel { get; set; }
        public ChannelCharactWindow(AirChannel airChannel)
        {
            InitializeComponent();
            AirChannel = airChannel;
            CopyCoeffs();
        }

        private bool CommitCoeffs()
        {
            try
            {
                double value = (double)PressureDropSupplyNumeric.Value;
                if (value < 0) throw new ArgumentException();
                AirChannel.EmptyChannelPressureDrop = value;
                return true;
            }
            catch(InvalidCastException )
            {
                MessageBox.Show("Wprowadz poprawne współczynniki");
                return false;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Wprowadz poprawne współczynniki");
                return false;
            }
        }

        private void CopyCoeffs()
        {
            PressureDropSupplyNumeric.Value = AirChannel.EmptyChannelPressureDrop;
        }

        private void OKbutton_Click(object sender, RoutedEventArgs e)
        {
            if(CommitCoeffs())
            {
                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
