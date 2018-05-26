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
    /// Interaction logic for ExchangerCharacteristicWindow.xaml
    /// </summary>
    public partial class ExchangerCharacteristicWindow : MetroWindow
    {
        private HVACExchanger Exchanger;
        public ExchangerCharacteristicWindow(HVACExchanger exchanger)
        {
            Exchanger = exchanger;
            InitializeComponent();

        }
    }
}
