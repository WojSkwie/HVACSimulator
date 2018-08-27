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
    /// Interaction logic for RoomCharactWindow.xaml
    /// </summary>
    public partial class RoomCharactWindow : MetroWindow
    {
        private HVACRoom HVACRoom;

        public RoomCharactWindow(HVACRoom room)
        {
            InitializeComponent();
            HVACRoom = room;
            CopyCoeffs();
        }

        private void OKbutton_Click(object sender, RoutedEventArgs e)
        {
            if(!ValidateInputs())
            {
                this.ShowMessageAsync("BŁĄD", "Wpisz poprawne współczynniki");
                return;
            }
            CommitCoeffs();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void CopyCoeffs()
        {
            VolumeNumeric.Value = HVACRoom.Volume;
            AirTempNumeric.Value = HVACRoom.AirInRoom.Temperature;
            AirHumidNumeric.Value = HVACRoom.AirInRoom.RelativeHumidity;
            WallAlphaNumeric.Value = HVACRoom.WallAlpha;
            WallHeatCapacityNumeric.Value = HVACRoom.WallHeatCapacity;
        }

        private void CommitCoeffs()
        {
            HVACRoom.Volume = (double)VolumeNumeric.Value;
            HVACRoom.AirInRoom.Temperature = (double)AirTempNumeric.Value;
            HVACRoom.AirInRoom.RelativeHumidity = (double)AirHumidNumeric.Value;
            HVACRoom.WallAlpha = (double)WallAlphaNumeric.Value;
            HVACRoom.WallHeatCapacity = (double)WallHeatCapacityNumeric.Value;
        }

        private bool ValidateInputs()
        {
            if (VolumeNumeric.Value == null || AirHumidNumeric.Value == null 
                || AirTempNumeric.Value == null || WallHeatCapacityNumeric.Value == null
                || WallAlphaNumeric.Value == null) return false;
            return true;
        }
    }
}
