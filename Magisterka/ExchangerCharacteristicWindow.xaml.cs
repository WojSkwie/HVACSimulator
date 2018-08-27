using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
    /// Interaction logic for ExchangerCharacteristicWindow.xaml
    /// </summary>
    public partial class ExchangerCharacteristicWindow : MetroWindow
    {
        private HVACExchanger Exchanger;
        private ExchangerParamsViewModel ExchangerParamsViewModel = new ExchangerParamsViewModel();
        public ExchangerCharacteristicWindow(HVACExchanger exchanger)
        {
            Exchanger = exchanger;
            InitializeComponent();
            CopyCoeffs();
        }

        private void CopyCoeffs()
        {
            AproxANumeric.Value = Exchanger.AproxA;
            AproxBNumeric.Value = Exchanger.AproxB;
            AproxCNumeric.Value = Exchanger.AproxC;
            AproxDNumeric.Value = Exchanger.AproxD;
            FreezingTimeNumeric.Value = Exchanger.SecondsToFreeze;
            MeltingTimeNumeric.Value = Exchanger.SecondsToMelt;

        }

        private void CommitCoeffs()
        {
            if (!CheckInput()) return;
            Exchanger.AproxA = (double)AproxANumeric.Value;
            Exchanger.AproxB = (double)AproxBNumeric.Value;
            Exchanger.AproxC = (double)AproxCNumeric.Value;
            Exchanger.AproxD = (double)AproxDNumeric.Value;
            Exchanger.SecondsToFreeze = (double)FreezingTimeNumeric.Value;
            Exchanger.SecondsToMelt = (double)MeltingTimeNumeric.Value;

        }

        private bool CheckInput()
        {
            if (AproxANumeric.Value == null || AproxBNumeric.Value == null 
                || AproxCNumeric.Value == null || AproxDNumeric.Value == null
                || FreezingTimeNumeric.Value == null || MeltingTimeNumeric.Value == null)
            {
                this.ShowMessageAsync("", "Wpisz współczynniki");
                return false;
            }
            return true;
        }

        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInput()) return;
            ExchangerParamsViewModel.ClearPlot();
            for(int i = 0; i < 60; i++)
            {
                double y = MathUtil.QubicEquaVal((double)AproxANumeric.Value, (double)AproxBNumeric.Value,
                    (double)AproxCNumeric.Value, (double)AproxDNumeric.Value, i / 50.0);

                ExchangerParamsViewModel.AddPoint(i / 50.0, y);
            }
            Plot.Model = ExchangerParamsViewModel.PlotModel;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            CommitCoeffs();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}