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
            MaximumEfficiencyNumeric.Value = Exchanger.MaximalEfficiencyPercent;
            DropoutCoeffNumeric.Value = Exchanger.EfficiencyDropoutCoefficient;
        }

        private void CommitCoeffs()
        {
            if (!CheckInput()) return;
            Exchanger.MaximalEfficiencyPercent = (double)MaximumEfficiencyNumeric.Value;
            Exchanger.EfficiencyDropoutCoefficient = (double)DropoutCoeffNumeric.Value;
        }

        private bool CheckInput()
        {
            if (MaximumEfficiencyNumeric.Value == null || DropoutCoeffNumeric.Value == null)
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
            for(int i = 0; i < 100; i++)
            {
                double y = Exchanger.UpdateSetEfficiency(i, (double)MaximumEfficiencyNumeric.Value, (double)DropoutCoeffNumeric.Value);
                ExchangerParamsViewModel.AddPoint(i, y);
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
