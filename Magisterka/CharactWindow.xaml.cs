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

namespace HVACSimulator
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
            SetVisuals();
            CopyCoeffs();
        }

        public CharacViewModel CharacModel { get; set; } = new CharacViewModel();

        private double CofA { get; set; }
        private double CofB { get; set; }
        private double CofC { get; set; }

        private double TimeConstant { get; set; }

        private void SetVisuals()
        {
            if(!currentObject.HasSingleTimeConstant)
            {
                TimeConstStackpanel.Visibility = Visibility.Collapsed;
            }
            if (!(currentObject is HVACFan))
            {
                CofCStackpanel.Visibility = Visibility.Collapsed;
            }
            if (currentObject is HVACTemperatureActiveObject)
            {
                MaxWaterFlowStackpanel.Visibility = Visibility.Visible;
            }
            /*if(currentObject is HVACCooler)
            {
                CoolerTemperatureStackpanel.Visibility = Visibility.Visible;
            }*/
        }

        private void CopyCoeffs()
        {
            CofATextBox.Text = currentObject.ACoeff.ToString();
            CofBTextBox.Text = currentObject.BCoeff.ToString();
            CofCTextBox.Text = currentObject.CCoeff.ToString();
            if(currentObject.HasSingleTimeConstant)
            {
                TimeConstTextBox.Text = currentObject.TimeConstant.ToString();
            }
            if (currentObject is HVACTemperatureActiveObject)
            {
                MaxWaterFlowTextBox.Text = ((HVACTemperatureActiveObject)currentObject).MaximalWaterFlow.ToString();
            }
            /*if(currentObject is HVACCooler)
            {
                CoolerTemperatureTextBox.Text = ((HVACCooler)currentObject).SurfaceTemperature.ToString();
            }*/
        }

        private bool CommitCoeffs()
        {
            try
            {
                currentObject.ACoeff = Convert.ToDouble(CofATextBox.Text);
                currentObject.BCoeff = Convert.ToDouble(CofBTextBox.Text);
                currentObject.CCoeff = Convert.ToDouble(CofCTextBox.Text);
                if (currentObject.HasSingleTimeConstant)
                {
                    currentObject.TimeConstant = Convert.ToDouble(TimeConstTextBox.Text);
                }
                if(currentObject is HVACTemperatureActiveObject)
                {
                    ((HVACTemperatureActiveObject)currentObject).MaximalWaterFlow = Convert.ToDouble(MaxWaterFlowTextBox.Text);
                }
                return true;
            }
            catch(InvalidCastException ex)
            {
                MessageBox.Show("Wpisz poprawne współczynniki");
                return false;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OKbutton_Click(object sender, RoutedEventArgs e)
        {
            if(CommitCoeffs())
            {
                DialogResult = true;
            }
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
                CofB = Convert.ToDouble(CofBTextBox.Text);
                CofC = Convert.ToDouble(CofCTextBox.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Wpisz poprawne liczby");
                return;
            }
            GetParabola(CofA, CofB, CofC, Constants.pointsOnCharac);            
        }

        private void GetParabola(double A, double B, double C, double Xmax)
        {
            CharacModel.ClearPlot();
            for(int i = 0; i < Xmax; i++)
            {
                double X = i;
                double Y = MathUtil.QuadEquaVal(A, B, C, X);
                if(Y > 0)
                {
                    CharacModel.AddPoint(X, Y);
                }
            }
            Plot.Model = CharacModel.PlotModel;
            
        }
    }
}
