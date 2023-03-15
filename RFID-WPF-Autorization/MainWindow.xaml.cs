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
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NFC.CardInserted += new NFCReader.CardEventHandler(Card_Inserted);
            //Ejected Event
            NFC.CardEjected += new NFCReader.CardEventHandler(CardRemoved);
            NFC.Watch();
            MessageBox.Show("We are in!", "CardReaded");
        }
        private void Card_Inserted()
        {

            if (NFC.Connect())
            {
                //Do stuff like NFC.GetCardUID(); ...
                Debug.WriteLine("Connected");
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    testtext.Text = NFC.GetCardUID();
                    Debug.WriteLine(NFC.ReadBlock("2"));
                    NFC.WriteBlock("Meme", "2"); // returns boolean
                    Debug.WriteLine(Encoding.UTF8.GetString(NFC.ReadBlock("2")));
                    NFC.GetReadersList();
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
