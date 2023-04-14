using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для EnteringPage.xaml
    /// </summary>
    public partial class EnteringPage : Page
    {
        NFCReader NFC = new NFCReader();
        List<HistoryModel> history = new List<HistoryModel>();
        List<WorkplaceReturnModel> workplaces = new List<WorkplaceReturnModel>();
        public ObservableCollection<string> list = new ObservableCollection<string>();
        UserModel loadeduserinfo = new UserModel();
        List<HistoryReturnModel> filteredhistory = new List<HistoryReturnModel>();
        int curentworkplaceid;
        public EnteringPage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
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
                curentworkplaceid = workplaces.FirstOrDefault(t => t.Name == CurrentWorkplace.SelectedItem.ToString()).id;
                await GetHistoryPeriod(new DateTime(1985, 3, 1,0,0,0),DateTime.Now.AddDays(1));
                foreach (HistoryModel item in history)
                {
                    await LoadUser(item.workerid);
                    filteredhistory.Add(new HistoryReturnModel { workerfio=loadeduserinfo.Name+" "+ loadeduserinfo.Surname+" "+loadeduserinfo.lastname, workplacename = workplaces.FirstOrDefault(t => t.id==item.workplaceid).Name,entertimestamp =item.entertimestamp.ToString("dd/MM/yyyy HH:mm:ss") });
                }
                ListBoxData.ItemsSource = filteredhistory.Where(t => t.workplacename == CurrentWorkplace.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("No reader connected.Please connect reader and reboot app", "CardReaded Error");
                Application.Current.Shutdown();

            }
        }


        private async Task GetHistoryPeriod(DateTime begin, DateTime end, int userid = 0)
        {
            string sqlformatbegin = begin.ToString("yyyy-MM-dd");
            string sqlformatend = end.ToString("yyyy-MM-dd");
            history = await ApiProcessor.getPeriodHistory(sqlformatbegin, sqlformatend, userid);
            
        }

        private async Task GetAlWorkplaces()
        {
            workplaces = await ApiProcessor.GetWorkplaceList();
        }

        private async Task LoadUser(int userid)
        {
            loadeduserinfo = await ApiProcessor.GetUser(userid);
        }

        private async Task CreateNewHistoryEntry(HistoryModel model)
        {
            var hostoryentry = await ApiProcessor.NewHistoryEntry(model);
        }

        private async Task Card_Inserted()
        {
            if (NFC.Connect())
            {
                await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(async () =>
                {
                    string userid = System.Text.Encoding.Default.GetString(NFC.ReadBlock("2"));

                    DateTime datenow = DateTime.Now;

                    if (int.TryParse(userid.Split("\0")[0], out var number))
                    {
                       
                        await CreateNewHistoryEntry(new HistoryModel
                        {
                            workerid = number,
                            workplaceid = curentworkplaceid,
                            entertimestamp = datenow
                        });
                        await LoadUser(number);
                        filteredhistory.Add(new HistoryReturnModel { workerfio = loadeduserinfo.Name + " " + loadeduserinfo.Surname + " " + loadeduserinfo.lastname, workplacename = workplaces.FirstOrDefault(t => t.id == curentworkplaceid).Name, entertimestamp = datenow.ToString("dd/MM/yyyy HH:mm:ss") });
                        filteredhistory=filteredhistory.OrderByDescending(o=>o.entertimestamp).ToList();
                        ListBoxData.ItemsSource = filteredhistory.Where(t => t.workplacename == CurrentWorkplace.Text);
                    }
                    else
                    { MessageBox.Show("Данные на карточке не были записанны", "Ошибка чтения"); }

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
            Debug.WriteLine("Card disconnected");
            NFC.Disconnect();
        }

        private void CurrentWorkplace_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxData.ItemsSource = filteredhistory.Where(t => t.workplacename == CurrentWorkplace.SelectedItem.ToString());
            curentworkplaceid = workplaces.FirstOrDefault(t => t.Name == CurrentWorkplace.SelectedItem.ToString()).id;
            MessageBox.Show($"Отслеживание прохода в отдел {CurrentWorkplace.SelectedItem.ToString()} номер в БД: {curentworkplaceid}", "Смена прохода");
        }
    }
}
