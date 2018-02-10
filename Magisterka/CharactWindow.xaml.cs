using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Magisterka
{
    /// <summary>
    /// Interaction logic for CharactWindow.xaml
    /// </summary>
    public partial class CharactWindow : MetroWindow
    {
        private HVACObject currentObject;
        public CharactWindow(HVACObject currentObject)
        {
            InitializeComponent();
            this.currentObject = currentObject;
        }

        private double CofA { get; set; }
        private double CofB { get; set; }
        private double CofC { get; set; }

        private double TimeConstant { get; set; }

        private void SetVisuals()
        {
            if (!(currentObject is HVACFan))
            {
                CofCTextBox.Visibility = Visibility.Collapsed;
                CofCLabel.Visibility = Visibility.Collapsed;
                TimeConstTextBox.Visibility = Visibility.Collapsed;
                TimeConstLabel.Visibility = Visibility.Collapsed;
            }
        }

        private void CopyCoeffs()
        {
            CofATextBox.Text = currentObject.ACoeff.ToString();
            CofBTextBox.Text = currentObject.BCoeff.ToString();
            CofCTextBox.Text = currentObject.CCoeff.ToString();
            if(currentObject is HVACFan)
            {
                TimeConstTextBox.Text = (currentObject as HVACFan).TimeConstant.ToString();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OKbutton_Click(object sender, RoutedEventArgs e)
        {
            CopyCoeffs();
            DialogResult = true;
        }

        private void CommitCoeffs()
        {
            throw new NotImplementedException();
        }

        private void NumericTextBox_Input(object sender, TextCompositionEventArgs e)
        {
            string input = e.Text;
            if(!IsInputNumerical(input, ((TextBox)sender).Text))
            {
                e.Handled = true;
            }
        }

        private bool IsInputNumerical(string inputText, string textInBox)
        {
            Regex regex = new Regex("[^0-9.,-]+");
            if (regex.IsMatch(inputText)) { return false; }
            if (textInBox.Contains(".") && (inputText.Contains(",") || inputText.Contains("."))) { return false; }
            if (textInBox.Contains(",") && (inputText.Contains(",") || inputText.Contains("."))) { return false; }
            return true;
        }

        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CofA = Convert.ToDouble(CofATextBox.Text);
                CofA = Convert.ToDouble(CofATextBox.Text);
                CofA = Convert.ToDouble(CofATextBox.Text);
            }
            catch(FormatException ex)
            {
                MessageBox.Show("Wpisz poprawne liczby");
            }
            
        }
    }
}
