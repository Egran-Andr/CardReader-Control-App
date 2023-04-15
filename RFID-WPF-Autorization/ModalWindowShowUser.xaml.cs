using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace RFID_WPF_Autorization
{
    /// <summary>
    /// Логика взаимодействия для ModalWindowShowUser.xaml
    /// </summary>
    public partial class ModalWindowShowUser : Window
    {
        int getid;
        private int timerTickCount = 0;
        private DispatcherTimer timer;
        UserModel showuser = new UserModel();
        public ModalWindowShowUser()
        {
            InitializeComponent();
        }

        public  ModalWindowShowUser(UserModel model,int userid)
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1); 
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
            showuser = model;
            getid = userid;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Loadimage(getid);
            FIOText.Content = showuser.Name + " " + showuser.Surname + " " + showuser.lastname;
            DateBirthText.Content += ": " + showuser.birthdate.ToString();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            if (++timerTickCount ==5)
            {
                timer.Stop();
                this.Close();
            }
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

        private async Task Loadimage(int userid)
        {
            var photo = await ApiProcessor.LoadImage(userid);
            UserImage.Source = ByteArrayToImage(photo);
        }
    }
}
