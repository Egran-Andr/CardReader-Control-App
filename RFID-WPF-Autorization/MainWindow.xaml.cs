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
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.IO;
using System.Drawing;
using System.Net;
using MaterialDesignThemes.Wpf;
using RFID_WPF_Autorization.Properties;
using System.Windows.Media.Animation;
using System.Threading;

namespace RFID_WPF_Autorization
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //NFCReader NFC = new NFCReader();
        public MainWindow()
        {
            InitializeComponent();
            ApiHelper.InitializeClient();
            PaletteHelper palette = new PaletteHelper();

            ITheme theme = palette.GetTheme();

            if (Settings.Default["Theme"].ToString() == "Light")
            {
                theme.SetBaseTheme(Theme.Light);
            }
            else
            {
                theme.SetBaseTheme(Theme.Dark);
            }
            palette.SetTheme(theme);

        }
        private static BitmapImage ByteArrayToImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }
        private async Task LoadUser(int userid)
        {
            var user = await ApiProcessor.GetUser(userid);
            MessageBox.Show(user.photopath, "userLoaded");
        }
        private async Task Createuser(UserModel user)
        {
            var newuser = await ApiProcessor.CreateUser(user);
            MessageBox.Show(newuser.id.ToString(), "userLoaded");
        }
        private async Task UpdateUser(int userid, UserModel user)
        {
            var newuser = await ApiProcessor.UpdateUser(userid, user);
        }
        private async Task Deleteuser(int userid)
        {
            var newuser = await ApiProcessor.DeleteUser(userid);
        }
        private async Task Loadimage(int userid)
        {
            var photo = await ApiProcessor.LoadImage(userid);
            //LoadImage.Source = ByteArrayToImage(photo);
        }
        private async Task PushImage(string filepath)
        {
            var photo = await ApiProcessor.UploadImage(filepath);

        }
        private async Task getUsers()
        {
            var userlist = await ApiProcessor.GetAllUsers();
        }

        private async Task CreateNewWorlplace(WorkplaceModel model)
        {
            var workplace = await ApiProcessor.CreateWorkplace(model);
        }

        private async Task CreateNewHistoryEntry(HistoryModel model)
        {
            var hostoryentry = await ApiProcessor.NewHistoryEntry(model);
        }

        private async Task CreateNewCardConnection(CardConnectionModel model)
        {
            var hostoryentry = await ApiProcessor.NewCardConnection(model);
        }

        private async Task GetHistoryById(int userid)
        {
            var userhistorylist = await ApiProcessor.GetUserHistory(userid);
        }

        private async Task GetConnectedCards(int userid = 0)
        {
            var connectedcardslist = await ApiProcessor.GetUserConnectedCards(userid);
        }

        private async Task GetHistoryPeriod(DateTime begin, DateTime end, int userid = 0)
        {
            string sqlformatbegin = begin.ToString("yyyy-MM-dd");
            string sqlformatend = end.ToString("yyyy-MM-dd");
            var periodhistory = await ApiProcessor.getPeriodHistory(sqlformatbegin, sqlformatend, userid);
            MessageBox.Show(periodhistory.Count.ToString());
        }

        private async Task DeletecardByName(string cardcode)
        {
            var periodhistory = await ApiProcessor.deleteCardconnection(cardcode);
        }

        private async Task GetAlWorkplaces()
        {
            var periodhistory = await ApiProcessor.GetWorkplaceList();
        }
        private async Task CreateNewWorkplaceConn(WorkplaceUserConnection connection)
        {
            var periodhistory = await ApiProcessor.CreateNewUserWorkspaceConnection(connection);
        }

        private async Task UpdateWorkplaceUserConn(WorkplaceUserConnection connection)
        {
            var periodhistory = await ApiProcessor.UpdateUserWorkplace(connection);
        }
        private async Task CreateNewGender(GenderModel gender)
        {
            var periodhistory = await ApiProcessor.CreateNewGender(gender);
        }

        private async Task CreateNewBalance(BalanceModel balance)
        {
            var periodhistory = await ApiProcessor.CreateNewBalance(balance);
        }
        private async Task UpdateBalance(BalanceModel balance)
        {
            var periodhistory = await ApiProcessor.UpdateUserBalance(balance);
        }
        private async Task GetUserBalance(int id)
        {
            var periodhistory = await ApiProcessor.getUserBalance(id);
            MessageBox.Show(periodhistory.balance.ToString(), periodhistory.workerid.ToString());
        }
        private async Task GetListBalances()
        {
            var balancelist = await ApiProcessor.getAllBalances();
            //MessageBox.Show(balancelist[0].balance.ToString(), balancelist[0].workerid.ToString());
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //NFC.CardInserted += new NFCReader.CardEventHandler(Card_Inserted);
                //Ejected Event
                //NFC.CardEjected += new NFCReader.CardEventHandler(CardRemoved);
                //NFC.Watch();
                if (Settings.Default["SaveLogsPath"].ToString() == "None")
                {
                    SettingsWindow Settings_Child_wimdow = new SettingsWindow(); ;
                    if (Settings_Child_wimdow.ShowDialog() == true)
                    {
                        _mainFrame.Navigate(new EnteringPage());
                    }
                }
                else
                {
                    if (Directory.Exists(Settings.Default["SaveLogsPath"].ToString()) == false)
                    {
                        string directory = Settings.Default["SaveLogsPath"].ToString();
                        MessageBox.Show($"Ошибка доступа к папке {directory}. Обновите директорию", "Ошибка доступа");
                        Settings.Default["SaveLogsPath"] = "None";
                        Settings.Default.Save();
                        SettingsWindow Settings_Child_wimdow = new SettingsWindow(); ;
                        if (Settings_Child_wimdow.ShowDialog() == true)
                        {
                            _mainFrame.Navigate(new EnteringPage());
                        }
                    }
                    else { _mainFrame.Navigate(new EnteringPage()); }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("No reader connected.Please connect reader and reboot app", "CardReaded Error");
                Application.Current.Shutdown();

            }

        }

        private bool _allowDirectNavigation = false;
        private NavigatingCancelEventArgs _navArgs = null;
        private Duration _duration = new Duration(TimeSpan.FromSeconds(0.3));
        private double _oldHeight = 0;
        private void frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (Content != null && !_allowDirectNavigation)
            {
                e.Cancel = true;

                _navArgs = e;
                _oldHeight = _mainFrame.ActualHeight;

                DoubleAnimation animation0 = new DoubleAnimation();
                animation0.From = _mainFrame.ActualHeight;
                animation0.To = 0;
                animation0.Duration = _duration;
                animation0.Completed += SlideCompleted;
                _mainFrame.BeginAnimation(HeightProperty, animation0);
            }
            _allowDirectNavigation = false;
        }

        private void SlideCompleted(object sender, EventArgs e)
        {
            _allowDirectNavigation = true;
            switch (_navArgs.NavigationMode)
            {
                case NavigationMode.New:
                    if (_navArgs.Uri == null)
                        _mainFrame.Navigate(_navArgs.Content);
                    else
                        _mainFrame.Navigate(_navArgs.Uri);
                    break;
                case NavigationMode.Back:
                    _mainFrame.GoBack();
                    break;
                case NavigationMode.Forward:
                    _mainFrame.GoForward();
                    break;
                case NavigationMode.Refresh:
                    _mainFrame.Refresh();
                    break;
            }

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                (ThreadStart)delegate ()
                {
                    DoubleAnimation animation0 = new DoubleAnimation();
                    animation0.From = 0;
                    animation0.To = _oldHeight;
                    animation0.Duration = _duration;
                    _mainFrame.BeginAnimation(HeightProperty, animation0);
                });
        }
    }
}
