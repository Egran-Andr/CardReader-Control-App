using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для UsersListPage.xaml
    /// </summary>
    public partial class UsersListPage : Page
    {
        public ObservableCollection<string> list = new ObservableCollection<string>();
        List<WorkplaceReturnModel> workplaces = new List<WorkplaceReturnModel>();
        NFCReader NFC = new NFCReader();
        int curentworkplaceid=0;

        public UsersListPage()
        {
            InitializeComponent();
            list.Add("Любой отдел");

        }
        private  async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                NFC.CardInserted += new NFCReader.CardEventHandler(Card_Inserted);
                //Ejected Event
                NFC.CardEjected += new NFCReader.CardEventHandler(CardRemoved);
                NFC.Watch();
                await GetAlWorkplaces();
                foreach (WorkplaceReturnModel item in workplaces)
                {
                    list.Add(item.Name);
                }
                CurrentWorkplace.ItemsSource = list;
                CurrentWorkplace.SelectedIndex = 0;
                //curentworkplaceid = workplaces.FirstOrDefault(t => t.Name == CurrentWorkplace.SelectedItem.ToString()).id;
                
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


        private async Task Card_Inserted()
        {
            if (NFC.Connect())
            {
                await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(async () =>
                {
                }));
            }
            else
            {
                //Give error message about connection...
                MessageBox.Show("Failed to find a reader connected to the system", "No reader connected");
            }
        }


        private async Task CardRemoved()
        {
            NFC.Disconnect();
        }


    }
}
