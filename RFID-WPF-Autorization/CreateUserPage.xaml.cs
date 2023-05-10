using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace RFID_WPF_Autorization
{
    /// <summary>
    /// Логика взаимодействия для CreateUserPage.xaml
    /// </summary>
    public partial class CreateUserPage : Page
    {
        string userimagefile= "default_user.jpeg";//полный путь к фотографии пользователя на компе клиента
        NFCReader NFC = new NFCReader();
        FullUser createduser = new FullUser();
        string cardid;
        UserModel user = new UserModel();
        bool createsignal = false;
        public CreateUserPage()
        {
            InitializeComponent();
        }

        private void UserNewImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter= "Image Files|*.jpg;*.jpeg;*.png;";
            //When the user select the file
            if (openFileDialog.ShowDialog() ==true )
            {
                userimagefile = openFileDialog.FileName;
                var converter = new ImageSourceConverter();
                UserNewImage.Source =
                    (ImageSource)converter.ConvertFromString(userimagefile);
            }
            Debug.WriteLine(UserNewImage.Source.ToString());
        }




        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                NFC.CardInserted += new NFCReader.CardEventHandler(Card_Inserted);
                //Ejected Event
                NFC.CardEjected += new NFCReader.CardEventHandler(CardRemoved);
                NFC.Watch();
            }
            catch (Exception)
            {
                MessageBox.Show("No reader connected.Please connect reader and reboot app", "CardReaded Error");
                Application.Current.Shutdown();

            }
        }

        private async Task Card_Inserted()
        {
            if (NFC.Connect())
            {
                await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(async () =>
                {
                    if (NameText.Text != "" & SurnameText.Text != "" & LocaleDatePicker.SelectedDate != null & genderbox.SelectedItem != null)//Все поля заполнены пользователем
                    {

                        user.Name = NameText.Text;
                        user.Surname = SurnameText.Text;
                        user.lastname = LastNameText.Text;
                        user.birthdate = LocaleDatePicker.DisplayDate.ToString("yyyy-MM-dd");
                        user.gender = genderbox.SelectedIndex + 1;

                            user.photopath = System.IO.Path.GetFileName(userimagefile);
                        await Createuser(user);
                        try { 
                            await CreateNewCardConnection(new CardConnectionModel { Userid = createduser.id, RFID_CardNumber = NFC.GetCardUID() });
                        }
                        catch (Exception e){
                            if (MessageBox.Show($"Карточка с номером {NFC.GetCardUID()} уже есть в системе.",
                    "Потдтвердить изменение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                await DeleteCardCon(NFC.GetCardUID());
                                await CreateNewCardConnection(new CardConnectionModel { Userid = createduser.id, RFID_CardNumber = NFC.GetCardUID()});
                                if (user.photopath != "default_user.jpeg") { await PushImage(userimagefile); }
                                NFC.WriteBlock(createduser.id.ToString(), "2");
                                MessageBox.Show("Пользователь успешно создан", "Успешно");
                                createsignal = true;
                                NFC.Disconnect();
                                ApiHelper.InitializeClient();
                                this.NavigationService.Navigate(new UsersListPage());
                            }
                            else
                            {
                                return;
                            }
                        };
                        if (createsignal == false)
                        {
                            if (user.photopath != "default_user.jpeg") { await PushImage(userimagefile); }
                            NFC.WriteBlock(createduser.id.ToString(), "2");
                            MessageBox.Show("Пользователь успешно создан", "Успешно");
                            NFC.Disconnect();
                            NFC.Dispose();
                            ApiHelper.InitializeClient();
                            this.NavigationService.Navigate(new UsersListPage());
                        }

                    }
                    else {
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

        private async Task Createuser(UserModel user)
        {
            var createdresponse = await ApiProcessor.CreateUser(user);
            createduser = createdresponse;
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

        private async Task PushImage(string filepath)
        {
            var photo = await ApiProcessor.UploadImage(filepath);
        }

        private async Task FullCreate(UserModel user, string filepath)
        {
            var createdresponse = await ApiProcessor.CreateUser(user);
            var photo = await ApiProcessor.UploadImage(filepath);
            var hostoryentry = await ApiProcessor.NewCardConnection(new CardConnectionModel { Userid = createdresponse.id, RFID_CardNumber = NFC.GetCardUID() });
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NFC.Disconnect();
            NFC.Dispose();
            this.NavigationService.Navigate(new UsersListPage());
        }
    }
}
