using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    /// Логика взаимодействия для UserUpdateDeleteModalWindow.xaml
    /// </summary>
    public partial class UserUpdateDeleteModalWindow : Window
    {
        FullUser localuser = new FullUser();
        UserModel user = new UserModel();
        public ObservableCollection<string> list = new ObservableCollection<string>();
        List<WorkplaceReturnModel> workplaces = new List<WorkplaceReturnModel>();
        WorkplaceModel modelwork = new WorkplaceModel();
        NFCReader NFC = new NFCReader();
        int workplacepos;
        int orgworkpos;
        bool createsignal = false;
        public UserUpdateDeleteModalWindow()
        {
            InitializeComponent();
            list.Add("Не назначено");

        }
        public UserUpdateDeleteModalWindow(FullUser user)
        {
            InitializeComponent();
            localuser = user;
            list.Add("Не назначено");
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
            if (workplacepos !=orgworkpos && workplacepos !=0)
            {
                if(orgworkpos == -1)//if user hadnt got any job-create new connection
                {
                    var item = workplaces.Where(t => t.Name.Equals(workbox.SelectedItem.ToString())).FirstOrDefault();
                    MessageBox.Show(item.id.ToString(), item.Name);
                    await CreateNewWorkplaceConn(new WorkplaceUserConnection { workerid=localuser.id,workplaceid=item.id});
                }
                else//update job if worker had any
                {
                    if(workplacepos != 0)
                    {
                        var item = workplaces.Where(t => t.Name.Equals(workbox.SelectedItem.ToString())).FirstOrDefault();
                        MessageBox.Show(item.id.ToString(), item.Name);
                        await UpdateWorkplaceUserConn(new WorkplaceUserConnection { workerid = localuser.id, workplaceid = item.id });
                    }
                }
            }
            await UpdateUser(localuser.id, user);
            NFC.Disconnect();
            NFC.Dispose();
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

        private async Task CreateNewWorkplaceConn(WorkplaceUserConnection connection)
        {
            var periodhistory = await ApiProcessor.CreateNewUserWorkspaceConnection(connection);
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

        private async Task UpdateWorkplaceUserConn(WorkplaceUserConnection connection)
        {
            var periodhistory = await ApiProcessor.UpdateUserWorkplace(connection);
        }

        private async Task UpdateUser(int userid, UserModel user)
        {
            var newuser = await ApiProcessor.UpdateUser(userid, user);
        }

        private async Task GetAlWorkplaces()
        {
            workplaces = await ApiProcessor.GetWorkplaceList();
        }

        private async Task GetUserWork(int userid)
        {
            modelwork = await ApiProcessor.getUserWorkplace(userid);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NFC.CardInserted += new NFCReader.CardEventHandler(Card_Inserted);
            //Ejected Event
            NFC.CardEjected += new NFCReader.CardEventHandler(CardRemoved);
            NFC.Watch();
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
            await GetAlWorkplaces();
            foreach (WorkplaceReturnModel item in workplaces)
            {
                list.Add(item.Name);
            }
            workbox.ItemsSource = list;
            await GetUserWork(localuser.id);
            int index = workbox.Items.IndexOf(modelwork.Name);
            if(index !=0)
            {
                workbox.SelectedItem = workbox.Items[index];
                orgworkpos= workbox.SelectedIndex;
                workplacepos = workbox.SelectedIndex;
            }
            else
            {
                workbox.SelectedIndex = 0;
                orgworkpos = -1;
                workplacepos = workbox.SelectedIndex;
            }
            
        }

        private void workbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            workplacepos = workbox.SelectedIndex;
        }

        private async Task CreateNewCardConnection(CardConnectionModel model)
        {
            try
            {
                var hostoryentry = await ApiProcessor.NewCardConnection(model);
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Card with this uid is already created");
            }
        }

        private async Task Card_Inserted()
        {
            if (NFC.Connect())
            {
                await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(async () =>
                {
                        try
                        {
                            await CreateNewCardConnection(new CardConnectionModel { Userid = localuser.id, RFID_CardNumber = NFC.GetCardUID() });
                        }
                        catch (Exception e)
                        {
                            if (MessageBox.Show($"Карточка с номером {NFC.GetCardUID()} уже есть в системе.",
                    "Потдтвердить изменение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                await DeleteCardCon(NFC.GetCardUID());
                                await CreateNewCardConnection(new CardConnectionModel { Userid = localuser.id, RFID_CardNumber = NFC.GetCardUID() });
                            NFC.WriteBlock(localuser.id.ToString(), "2");
                                MessageBox.Show("Карточка перезаписанна", "Успешно");
                                createsignal = true;
                                NFC.Disconnect();
                            }
                            else
                            {
                                return;
                            }
                        };
                        if (createsignal == false)
                        {
                            NFC.WriteBlock(localuser.id.ToString(), "2");
                            MessageBox.Show("Пользователь успешно создан", "Успешно");
                            NFC.Disconnect();
                        }

                    
                    else
                    {
                        MessageBox.Show("Заполните все поля. Проверьте правильность ввода");
                    }

                }));
            }
            else
            {
                //Give error message about connection...
                MessageBox.Show("Failed to find a reader connected to the system", "No reader connected");
            }
        }
        private async Task DeleteCardCon(string v)
        {
            try
            {
                var createdresponse = await ApiProcessor.deleteCardconnection(v);
            }
            catch (Exception e)
            {
                throw new ArgumentException($"CreatingError");
            }

        }

        private async Task CardRemoved()
        {
            Debug.WriteLine("Card disconnected");

        }
    }
}
