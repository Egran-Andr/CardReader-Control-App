﻿using System;
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

namespace RFID_WPF_Autorization
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NFCReader NFC = new NFCReader();
        public MainWindow()
        {
            InitializeComponent();
            ApiHelper.InitializeClient();
            
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
            MessageBox.Show(user.photopath, "userLoaded");
        }
        private async Task UpdateUser(int userid,UserModel user)
        {
            var newuser = await ApiProcessor.UpdateUser(userid,user);
        }
        private async Task Deleteuser(int userid)
        {
            var newuser = await ApiProcessor.DeleteUser(userid);
        }
        private async Task Loadimage(int userid)
        {
            var photo = await ApiProcessor.LoadImage(userid);
            LoadImage.Source = ByteArrayToImage(photo);
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

        private async Task GetConnectedCards(int userid=0)
        {
            var connectedcardslist = await ApiProcessor.GetUserConnectedCards(userid);
        }

        private async Task GetHistoryPeriod(DateTime begin,DateTime end, int userid = 0)
        {
            string sqlformatbegin= begin.ToString("yyyy-MM-dd");
            string sqlformatend= end.ToString("yyyy-MM-dd");
            var periodhistory = await ApiProcessor.getPeriodHistory(sqlformatbegin, sqlformatend, userid);
            MessageBox.Show(periodhistory.Count.ToString());
        }

        private async Task DeletecardByName(string cardcode)
        {
            var periodhistory = await ApiProcessor.deleteCardconnection(cardcode);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NFC.CardInserted += new NFCReader.CardEventHandler(Card_Inserted);
            //Ejected Event
            NFC.CardEjected += new NFCReader.CardEventHandler(CardRemoved);
            NFC.Watch();
            MessageBox.Show("We are in!", "CardReaded");

            UserModel user = new UserModel { 
                Name="Евгений",
                Surname="Васильков",
                lastname="Александрович",
                birthdate="2022-01-10",
                gender=1,
                photopath="string"
            };
            WorkplaceModel model = new WorkplaceModel { Name = "Test" };
            HistoryModel modelhistory = new HistoryModel { workerid=3,workplaceid=1, entertimestamp=DateTime.Now};

            //await Createuser(user);
            //await LoadUser(4);
            //await UpdateUser(1,user);
            //await Deleteuser(1);
            //await Loadimage(5);
            //await PushImage("C:/Users/User/Downloads/1cid8j6nUmw.jpg");
            //await getUsers();
            //await CreateNewWorlplace(model);
            //await CreateNewHistoryEntry(modelhistory);
            //await GetHistoryById(3);
            //await GetConnectedCards();
            //await GetHistoryPeriod(new DateTime(2022, 1, 1, 0, 0, 0), new DateTime(2023, 4, 5, 0, 0, 0),4);
            //await DeletecardByName("string");
        }
        private async void Card_Inserted()
        {
            
            if (NFC.Connect())
            {
                //Do stuff like NFC.GetCardUID(); ...
                Debug.WriteLine("Connected");
                await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(async () =>
                 {
                     testtext.Text = NFC.GetCardUID();
                     Debug.WriteLine(NFC.ReadBlock("2"));
                     NFC.WriteBlock("Meme", "2"); // returns boolean
                     Debug.WriteLine(Encoding.UTF8.GetString(NFC.ReadBlock("2")));
                     NFC.GetReadersList();
                     //CardConnectionModel cardConnection = new CardConnectionModel { Userid = 3, RFID_CardNumber = NFC.GetCardUID() };
                     //await CreateNewCardConnection(cardConnection);
                 }));
            }
            else
            {
                //Give error message about connection...
                MessageBox.Show("Failed to find a reader connected to the system", "No reader connected");
            }
        }

        private void CardRemoved()
        {
            Debug.WriteLine("Disconected");
            NFC.Disconnect();
        }

    }
}
