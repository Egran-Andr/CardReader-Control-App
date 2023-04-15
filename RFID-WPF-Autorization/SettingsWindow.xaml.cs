using MaterialDesignThemes.Wpf;
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
using RFID_WPF_Autorization.Properties;
using System.Windows.Threading;
using Microsoft.Win32;

namespace RFID_WPF_Autorization
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private DispatcherTimer timer;
        private int timerTickCount = 0;
        public SettingsWindow()
        {
            InitializeComponent();
            if (Settings.Default["Theme"].ToString() == "Light")
            {
                ToogleTheme.IsChecked = false;
            }
            else
            {
                ToogleTheme.IsChecked = true;
            }
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += new EventHandler(Timer_Tick);
        }

        private void ToogleTheme_Click(object sender, RoutedEventArgs e)
        {
            PaletteHelper palette = new PaletteHelper();

            ITheme theme = palette.GetTheme();


            if (ToogleTheme.IsChecked == true)
            {
                theme.SetBaseTheme(Theme.Dark);
                Settings.Default["Theme"] = "Dark";
            }
            else
            {
                theme.SetBaseTheme(Theme.Light);
                Settings.Default["Theme"] = "Light";
            }
            palette.SetTheme(theme);
            Settings.Default.Save();
            ThemeSwitchSback.IsActive = true;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            if (++timerTickCount == 1)
            {
                timer.Stop();
                ThemeSwitchSback.IsActive = false;
                timerTickCount = 0;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           
        }
    }
}
