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

namespace RFID_WPF_Autorization
{
    /// <summary>
    /// Логика взаимодействия для UserUpdateDeleteModalWindow.xaml
    /// </summary>
    public partial class UserUpdateDeleteModalWindow : Window
    {
        FullUser localuser = new FullUser();
        UserModel user = new UserModel();
        public UserUpdateDeleteModalWindow()
        {
            InitializeComponent();
        }
        public UserUpdateDeleteModalWindow(FullUser user)
        {
            InitializeComponent();
            localuser = user;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult =false;
            this.Close();
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            user.Name = NameText.Text;
            user.Surname = SurnameText.Text;
            user.lastname = LastNameText.Text;
            user.birthdate = LocaleDatePicker.DisplayDate.ToString("yyyy-MM-dd");
            user.gender = genderbox.SelectedIndex + 1;
            user.photopath = localuser.photopath;
            await UpdateUser(localuser.id, user);
            this.DialogResult = true;
            this.Close();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены что хотите удалить данного пользователя из системы?", "Удаление пользователя", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                await Deleteuser(localuser.id);
                this.DialogResult = true;
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
        private async Task Deleteuser(int userid)
        {
            var newuser = await ApiProcessor.DeleteUser(userid);
        }

        private async Task UpdateUser(int userid, UserModel user)
        {
            var newuser = await ApiProcessor.UpdateUser(userid, user);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NameText.Text = localuser.Name;
            SurnameText.Text = localuser.Surname;
            LastNameText.Text = localuser.lastname;
            LocaleDatePicker.SelectedDate = DateTime.Parse(localuser.birthdate);
            if (localuser.gender==1) { 
                genderbox.SelectedIndex = 0;
            }
            else
            {
                genderbox.SelectedIndex = 1;
            }
            await Loadimage(localuser.id);
        }
    }
}
