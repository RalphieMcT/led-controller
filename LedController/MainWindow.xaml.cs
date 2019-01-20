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
using System.Windows.Forms;
using System.IO.Ports;
using System.Drawing;


namespace LedController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        public string selectedComPort;
        public SerialPort serialPort;
        public struct PinState{ public int a; public int b; public int c; public int d; }
        public PinState currentPinStates;

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            foreach (string comPort in SerialPort.GetPortNames())
            {
                var cboxItem = new ComboBoxItem();
                cboxItem.Content = comPort;
                ComPortComboBox.Items.Add(cboxItem);
                selectedComPort = comPort;
                serialPort = new SerialPort(selectedComPort, 115200, Parity.None, 8, StopBits.One);
            }
            System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = new System.Drawing.Icon("icon.ico");
            ni.Visible = true;
            ni.Click +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
            Ch1_Slider.Maximum = 255;
            Ch1_Slider.Minimum = 0;
            Ch2_Slider.Maximum = 255;
            Ch2_Slider.Minimum = 0;
            Ch3_Slider.Maximum = 255;
            Ch3_Slider.Minimum = 0;
            Ch4_Slider.Maximum = 255;
            Ch4_Slider.Minimum = 0;
            currentPinStates = new PinState();

            //init defaults
            Ch1_Slider.Value = Ch1_Slider.Maximum; //PA8 MOSFET
            Ch3_Slider.Value = Ch3_Slider.Maximum; //PB9 LED
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
        }

        private void Button_Click_Command(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(CommandTextBox.Text) == false)
            {
                if (serialPort.IsOpen == false)
                {
                    serialPort.Open();
                }

                string command = CommandTextBox.Text + '\r';
                char[] data = command.ToCharArray();
                try
                {
                    serialPort.Write(data, 0, data.Length);
                }
                catch (UnauthorizedAccessException ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }
        
        private void ComPortCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedComPort = ((ComboBoxItem)ComPortComboBox.SelectedItem).Content.ToString();
            serialPort = new SerialPort(selectedComPort, 115200, Parity.None, 8, StopBits.One);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            PinState newPinStates = currentPinStates;
            if (slider.Name == "Ch1_Slider")
            {
                newPinStates.a = (int)slider.Value;
            }
            else if (slider.Name == "Ch2_Slider")
            {
                newPinStates.b = (int)slider.Value;
            }
            else if (slider.Name == "Ch3_Slider")
            {
                newPinStates.c = (int)slider.Value;
            }
            else if (slider.Name == "Ch4_Slider")
            {
                newPinStates.d = (int)slider.Value;
            }
            FadeToNewValues(newPinStates);
        }

        private void FadeToNewValues(PinState newPinStates)
        {
            if (serialPort.IsOpen == false)
                serialPort.Open();

            while (newPinStates.a != currentPinStates.a || newPinStates.b != currentPinStates.b || newPinStates.c != currentPinStates.c || newPinStates.d != currentPinStates.d)
            {
                if (newPinStates.a != currentPinStates.a)
                {
                    currentPinStates.a += currentPinStates.a > newPinStates.a ? -1 : 1;
                }
                if (newPinStates.b != currentPinStates.b)
                {
                    currentPinStates.b += currentPinStates.b > newPinStates.b ? -1 : 1;
                }
                if (newPinStates.c != currentPinStates.c)
                {
                    currentPinStates.c += currentPinStates.c > newPinStates.c ? -1 : 1;
                }
                if (newPinStates.d != currentPinStates.d)
                {
                    currentPinStates.d += currentPinStates.d > newPinStates.d ? -1 : 1;
                }
                string data = string.Format("{0},{1},{2},{3}", currentPinStates.a, currentPinStates.b, currentPinStates.c, currentPinStates.d) + '\r';
                try
                {
                    serialPort.Write(data.ToCharArray(), 0, data.Length);
                }
                catch (UnauthorizedAccessException e)
                {
                    System.Windows.MessageBox.Show(e.Message);
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Ch1_0(object sender, RoutedEventArgs e)
        {
            Ch1_Slider.Value = Ch1_Slider.Minimum;
        }

        private void Ch1_100(object sender, RoutedEventArgs e)
        {
            Ch1_Slider.Value = Ch1_Slider.Maximum;
        }

        private void Ch2_0(object sender, RoutedEventArgs e)
        {
            Ch2_Slider.Value = Ch2_Slider.Minimum;
        }

        private void Ch2_100(object sender, RoutedEventArgs e)
        {
            Ch2_Slider.Value = Ch2_Slider.Maximum;
        }

        private void Ch3_0(object sender, RoutedEventArgs e)
        {
            Ch3_Slider.Value = Ch3_Slider.Minimum;
        }

        private void Ch3_100(object sender, RoutedEventArgs e)
        {
            Ch3_Slider.Value = Ch3_Slider.Maximum;
        }

        private void Ch4_0(object sender, RoutedEventArgs e)
        {
            Ch4_Slider.Value = Ch4_Slider.Minimum;
        }

        private void Ch4_100(object sender, RoutedEventArgs e)
        {
            Ch4_Slider.Value = Ch4_Slider.Maximum;
        }

        private void CommandTextBox_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (CommandTextBox.Text.Trim() != "" || CommandTextBox.Text != null)
            {
                CommandTextBox.Text = "";
            }
        }
    }
}
