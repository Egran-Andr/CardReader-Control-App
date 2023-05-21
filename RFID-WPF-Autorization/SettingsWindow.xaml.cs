using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using RFID_WPF_Autorization.Properties;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace RFID_WPF_Autorization
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private DispatcherTimer timer;
        private int timerTickCount = 0;
        bool foulderselected = false;
        private string selectedsavepath;
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
            if (Settings.Default["SaveLogsPath"].ToString() != "None")
            {
                foulderselected = true;
                selectedsavepath = Settings.Default["SaveLogsPath"].ToString();
                SaveLabel.Content = selectedsavepath;
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
            if (foulderselected == true)
            {
                Settings.Default["SaveLogsPath"] = selectedsavepath;
                Settings.Default.Save();
                this.DialogResult = true;
                this.Close();
            }
            else { MessageBox.Show("Заполните поле для сохранения", "Ошибка заполнения"); }

        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SaveLabel.Content = dialog.FileName;
                selectedsavepath = dialog.FileName;
                foulderselected = true;
                MessageBox.Show($"Директория сохранения файлов для логов изменена:{selectedsavepath}", "Изменение директории");
            }
            this.Activate();
        }
    }
}
