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

namespace RFID_WPF_Autorization
{
    /// <summary>
    /// Логика взаимодействия для HistoryExportPage.xaml
    /// </summary>
    public partial class HistoryExportPage : Page
    {
        List<HistoryModel> history = new List<HistoryModel>();
        List<WorkplaceReturnModel> workplaces = new List<WorkplaceReturnModel>();
        public ObservableCollection<string> list = new ObservableCollection<string>();
        UserModel loadeduserinfo = new UserModel();
        List<HistoryReturnModelDate> filteredhistory = new List<HistoryReturnModelDate>();
        List<HistoryReturnModelDate> historycopy = new List<HistoryReturnModelDate>();
        int curentworkplaceid;
        public HistoryExportPage()
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
                Workplacefilter.ItemsSource = list;
                Workplacefilter.SelectedIndex = 0;
                await GetHistoryPeriod(DateTime.MinValue, DateTime.MaxValue);
                foreach (HistoryModel item in history)
                {
                    await LoadUser(item.workerid);
                    filteredhistory.Add(new HistoryReturnModelDate { workerfio = loadeduserinfo.Name + " " + loadeduserinfo.Surname + " " + loadeduserinfo.lastname, workplacename = workplaces.FirstOrDefault(t => t.id == item.workplaceid).Name, entertimestamp = item.entertimestamp});
                }
                BeginDate.SelectedDate = DateTime.Now.AddYears(-20);
                EndDate.SelectedDate = DateTime.Now.AddDays(1);
                HistorydataGrid.ItemsSource = filteredhistory;
                ExportNumber.Text = $"Количество записей для импорта: {filteredhistory.Count}";
            }
            catch (Exception ex)
            {
                if (ex.Message == "Not Found")
                {
                    HistorydataGrid.ItemsSource = filteredhistory;
                }
                else
                {
                    MessageBox.Show("No reader connected.Please connect reader and reboot app", "CardReaded Error");
                }

            }
        }

        private async Task GetAlWorkplaces()
        {
            workplaces = await ApiProcessor.GetWorkplaceList();
        }

        private async Task GetHistoryPeriod(DateTime begin, DateTime end, int userid = 0)
        {
            string sqlformatbegin = begin.ToString("yyyy-MM-dd");
            string sqlformatend = end.ToString("yyyy-MM-dd");
            history = await ApiProcessor.getPeriodHistory(sqlformatbegin, sqlformatend, userid);

        }

        private async Task LoadUser(int userid)
        {
            loadeduserinfo = await ApiProcessor.GetUser(userid);
        }

        private void FioText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Workplacefilter.SelectedIndex == 0) {

                if(BeginDate.SelectedDate.HasValue && EndDate.SelectedDate.HasValue)
                {
                    HistorydataGrid.ItemsSource = filteredhistory.Where(t => t.entertimestamp >= BeginDate.SelectedDate.Value && t.entertimestamp <= EndDate.SelectedDate.Value && t.workerfio.ToLower().Contains(FioText.Text.ToLower()));
                    historycopy = filteredhistory.Where(t => t.entertimestamp >= BeginDate.SelectedDate.Value && t.entertimestamp <= EndDate.SelectedDate.Value && t.workerfio.ToLower().Contains(FioText.Text.ToLower())).ToList();
                }
                else
                {
                    HistorydataGrid.ItemsSource = filteredhistory.Where(t => t.workerfio.ToLower().Contains(FioText.Text.ToLower()));
                    historycopy = filteredhistory.Where(t => t.workerfio.ToLower().Contains(FioText.Text.ToLower())).ToList();
                }

            }
            else
            {

                if (BeginDate.SelectedDate.HasValue && EndDate.SelectedDate.HasValue)
                {
                    HistorydataGrid.ItemsSource = filteredhistory.Where(t => t.entertimestamp >= BeginDate.SelectedDate.Value && t.entertimestamp <= EndDate.SelectedDate.Value && t.workplacename.Equals(Workplacefilter.SelectedItem.ToString()) && t.workerfio.ToLower().Contains(FioText.Text.ToLower()));
                    historycopy = filteredhistory.Where(t => t.entertimestamp >= BeginDate.SelectedDate.Value && t.entertimestamp <= EndDate.SelectedDate.Value && t.workplacename.Equals(Workplacefilter.SelectedItem.ToString()) && t.workerfio.ToLower().Contains(FioText.Text.ToLower())).ToList();
                }
                else
                {
                    HistorydataGrid.ItemsSource = filteredhistory.Where(t => t.workplacename.Equals(Workplacefilter.SelectedItem.ToString()) && t.workerfio.ToLower().Contains(FioText.Text.ToLower()));
                    historycopy = filteredhistory.Where(t => t.workplacename.Equals(Workplacefilter.SelectedItem.ToString()) && t.workerfio.ToLower().Contains(FioText.Text.ToLower())).ToList();
                }
                
            }
            ExportNumber.Text = $"Количество записей для импорта: {historycopy.Count}";
        }

        private  void BeginDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EndDate.SelectedDate != null) {
                if (Workplacefilter.SelectedIndex == 0)
                {
                    HistorydataGrid.ItemsSource = filteredhistory.Where(t => t.entertimestamp >= BeginDate.SelectedDate.Value && t.entertimestamp <= EndDate.SelectedDate.Value  && t.workerfio.ToLower().Contains(FioText.Text.ToLower()));
                    historycopy = filteredhistory.Where(t => t.entertimestamp >= BeginDate.SelectedDate.Value && t.entertimestamp <= EndDate.SelectedDate.Value && t.workerfio.ToLower().Contains(FioText.Text.ToLower())).ToList();
                }
                else
                {
                    HistorydataGrid.ItemsSource = filteredhistory.Where(t => t.entertimestamp >= BeginDate.SelectedDate.Value && t.entertimestamp <= EndDate.SelectedDate.Value && t.entertimestamp >= BeginDate.SelectedDate.Value && t.entertimestamp <= EndDate.SelectedDate.Value);
                    historycopy = filteredhistory.Where(t => t.entertimestamp >= BeginDate.SelectedDate.Value && t.entertimestamp <= EndDate.SelectedDate.Value && t.entertimestamp >= BeginDate.SelectedDate.Value && t.entertimestamp <= EndDate.SelectedDate.Value).ToList();
                }

                ExportNumber.Text = $"Количество записей для импорта: {historycopy.Count}";
            }
            else
            {
            }
        }

        private  void EndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BeginDate.SelectedDate != null)
            {
                if (Workplacefilter.SelectedIndex == 0)
                {
                    HistorydataGrid.ItemsSource = filteredhistory.Where(t => t.entertimestamp >= BeginDate.SelectedDate.Value && t.entertimestamp <= EndDate.SelectedDate.Value && t.workerfio.ToLower().Contains(FioText.Text.ToLower()));
                    historycopy = filteredhistory.Where(t => t.entertimestamp >= BeginDate.SelectedDate.Value && t.entertimestamp <= EndDate.SelectedDate.Value && t.workerfio.ToLower().Contains(FioText.Text.ToLower())).ToList();
                }
                else
                {
                    HistorydataGrid.ItemsSource = filteredhistory.Where(t => t.entertimestamp >= BeginDate.SelectedDate.Value && t.entertimestamp <= EndDate.SelectedDate.Value && t.entertimestamp >= BeginDate.SelectedDate.Value && t.entertimestamp <= EndDate.SelectedDate.Value);
                    historycopy = filteredhistory.Where(t => t.entertimestamp >= BeginDate.SelectedDate.Value && t.entertimestamp <= EndDate.SelectedDate.Value && t.entertimestamp >= BeginDate.SelectedDate.Value && t.entertimestamp <= EndDate.SelectedDate.Value).ToList();
                }

                ExportNumber.Text = $"Количество записей для импорта: {historycopy.Count}";
            }
            else
            {
               
            }
        }

        private void Workplacefilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Workplacefilter.SelectedIndex == 0)
            {
                if (BeginDate.SelectedDate.HasValue && EndDate.SelectedDate.HasValue)
                {
                    HistorydataGrid.ItemsSource = filteredhistory.Where(t => t.entertimestamp >= BeginDate.SelectedDate.Value && t.entertimestamp <= EndDate.SelectedDate.Value && t.workerfio.ToLower().Contains(FioText.Text.ToLower()));
                    historycopy = filteredhistory.Where(t => t.entertimestamp >= BeginDate.SelectedDate.Value && t.entertimestamp <= EndDate.SelectedDate.Value && t.workerfio.ToLower().Contains(FioText.Text.ToLower())).ToList();
                }
                else
                {
                    HistorydataGrid.ItemsSource = filteredhistory.Where(t => t.workplacename.Equals(Workplacefilter.SelectedItem.ToString()) && t.workerfio.ToLower().Contains(FioText.Text.ToLower()));
                    historycopy = filteredhistory.Where(t => t.workplacename.Equals(Workplacefilter.SelectedItem.ToString()) && t.workerfio.ToLower().Contains(FioText.Text.ToLower())).ToList();
                }
            }
            else
            {
                if (BeginDate.SelectedDate.HasValue && EndDate.SelectedDate.HasValue)
                {
                    HistorydataGrid.ItemsSource = filteredhistory.Where(t => t.entertimestamp >= BeginDate.SelectedDate.Value && t.entertimestamp <= EndDate.SelectedDate.Value && t.workerfio.ToLower().Contains(FioText.Text.ToLower()) && t.workplacename.Equals(Workplacefilter.SelectedItem.ToString()));
                    historycopy = filteredhistory.Where(t => t.entertimestamp >= BeginDate.SelectedDate.Value && t.entertimestamp <= EndDate.SelectedDate.Value && t.workplacename.Equals(Workplacefilter.SelectedItem.ToString()) && t.workerfio.ToLower().Contains(FioText.Text.ToLower())).ToList();
                }
                else
                {
                    HistorydataGrid.ItemsSource = filteredhistory.Where(t => t.workplacename.Equals(Workplacefilter.SelectedItem.ToString()) && t.workerfio.ToLower().Contains(FioText.Text.ToLower()) && t.workplacename.Equals(Workplacefilter.SelectedItem.ToString()));
                    historycopy = filteredhistory.Where(t => t.workplacename.Equals(Workplacefilter.SelectedItem.ToString()) && t.workerfio.ToLower().Contains(FioText.Text.ToLower())).ToList();
                }

            }
            ExportNumber.Text = $"Количество записей для импорта: {historycopy.Count}";
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

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
