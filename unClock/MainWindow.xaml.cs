using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Shapes;

namespace unClock
{
    [Flags]
    public enum EXECUTION_STATE : uint
    {
        ES_AWAYMODE_REQUIRED = 0x00000040,
        ES_CONTINUOUS = 0x80000000,
        ES_DISPLAY_REQUIRED = 0x00000002,
        ES_SYSTEM_REQUIRED = 0x00000001
        // Legacy flag, should not be used.
        // ES_USER_PRESENT = 0x00000004
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [DllImport("user32.dll")]
        static extern bool LockWorkStation();

        public MainWindow()
        {
            InitializeComponent();

            SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);

            UpdateClocks();
        }

        private async void UpdateClocks()
        {
            while (true)
            {
                Instant now = SystemClock.Instance.GetCurrentInstant();

                var tkyNow = now.InZone(DateTimeZoneProviders.Tzdb["Asia/Tokyo"]);
                var ldnNow = now.InZone(DateTimeZoneProviders.Tzdb["Europe/London"]);

                this.TbTokyo.Text = tkyNow.ToString("HH:mm", null);
                this.TbLondon.Text = ldnNow.ToString("HH:mm", null);

                await Task.Delay(500);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnLock_Click(object sender, RoutedEventArgs e)
        {
            LockWorkStation();
        }
    }

    
}
