using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace RFID_WPF_Autorization
{
    /// <summary>
    /// Логика взаимодействия для UsersListPage.xaml
    /// </summary>
    public partial class UsersListPage : Page
    {
        public ObservableCollection<string> list = new ObservableCollection<string>();
        List<WorkplaceReturnModel> workplaces = new List<WorkplaceReturnModel>();
        List<WorkplaceUserConnection> userworkplace = new List<WorkplaceUserConnection>();
        List<FullUser> users = new List<FullUser>();
        ObservableCollection<FullUserReturn> usersloaded = new ObservableCollection<FullUserReturn>();
        WorkplaceModel modelwork = new WorkplaceModel();
        int curentworkplaceid = 0;
        BitmapImage localimage;

        public UsersListPage()
        {
            InitializeComponent();
            list.Add("Любой отдел");

        }


        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await GetAlWorkplaces();
                foreach (WorkplaceReturnModel item in workplaces)
                {
                    list.Add(item.Name);
                }
                CurrentWorkplace.ItemsSource = list;
                CurrentWorkplace.SelectedIndex = 0;
                await getUsers();
                double percent = 100 / (double)users.Count();
                foreach (FullUser item in users)
                {
                    AnimatedProgressInCard.Value += percent;
                    await Loadimage(item.id);
                    await getWorkplace(item.id);
                    usersloaded.Add(new FullUserReturn { id = item.id, workerfio = item.Name + " " + item.Surname + " " + item.lastname, workplacename = modelwork.Name, gender = item.gender, datebirth = DateTime.ParseExact(item.birthdate, "yyyy-MM-dd", null), photopath = localimage });
                }
                UserDataGrid.ItemsSource = usersloaded;
                AnimatedProgressInCard.Visibility = System.Windows.Visibility.Collapsed;

            }
            catch (Exception ex)
            {
                MessageBox.Show("No reader connected.Please connect reader and reboot app", "CardReaded Error");
                Application.Current.Shutdown();
            }
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new CreateUserPage());
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow Settings_Child_wimdow = new SettingsWindow();
            Settings_Child_wimdow.ShowDialog();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new EnteringPage());
        }

        private void CurrentWorkplace_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private async Task GetAlWorkplaces()
        {
            workplaces = await ApiProcessor.GetWorkplaceList();
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
            localimage = ByteArrayToImage(photo);

        }

        private async Task getUsers()
        {
            users = await ApiProcessor.GetAllUsers();
        }
        private async Task getWorkplace(int userid)
        {
            modelwork = await ApiProcessor.getUserWorkplace(userid);
        }


        private void UserDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserDataGrid.SelectedItem != null)
            {
                FullUserReturn clickeduser = (FullUserReturn)UserDataGrid.SelectedItem;
                UserUpdateDeleteModalWindow userDialog = new UserUpdateDeleteModalWindow(users.Where(i => i.id.Equals(clickeduser.id)).First());
                if (userDialog.ShowDialog() == true)
                {
                    this.NavigationService.Refresh();
                }
            }

        }


        private void CurrentWorkplace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserDataGrid.SelectedItem = null;
            if (CurrentWorkplace.SelectedIndex == 0) { UserDataGrid.ItemsSource = usersloaded; }
            else
            {
                UserDataGrid.ItemsSource = usersloaded.Where(t => t.workplacename.Equals(CurrentWorkplace.SelectedItem.ToString()));
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UserDataGrid.SelectedItem = null;
            if (CurrentWorkplace.SelectedIndex == 0) { UserDataGrid.ItemsSource = usersloaded.Where(t => t.workerfio.ToLower().Contains(FioText.Text.ToLower())); }
            else
            {
                UserDataGrid.ItemsSource = usersloaded.Where(t => t.workplacename.Equals(CurrentWorkplace.SelectedItem.ToString()) && t.workerfio.ToLower().Contains(FioText.Text.ToLower()));
            }
        }
    }
}
