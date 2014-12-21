using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using SnifferMC.Resourse;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace SnifferMC
{
    public class ViewModel : ViewModelBase
    {
        private SessionSave _session = SessionSave.Instance; //for save information
        private System.Object lockThis = new System.Object(); // for lock
        private NetworkInterface[] adapters;// list of Net adapters
        private NetworkInterface adapter;
        private DispatcherTimer timer;// taimer for update information
        private DispatcherTimer timerForChart;// taimer for update information
        //  private const double timerUpdate = 1000; //taimer update tame
        private static volatile bool statusOfWork = true; //status of update information in Tabel
        private static List<IPAddreData> _listViewBoxBuffer = new List<IPAddreData>();//buffering information before insert in the table
        private static ObservableCollection<IPAddreData> _listViewBox = new ObservableCollection<IPAddreData>();//Table listener
        private ObservableCollection<String> _comboBoxList = new ObservableCollection<String>(); //information for comBoBox whit Net adapters

        private ObservableCollection<KeyValuePair<string, long>> _loadData = new ObservableCollection<KeyValuePair<string, long>>();
        private ObservableCollection<KeyValuePair<string, long>> _sendData = new ObservableCollection<KeyValuePair<string, long>>();
       
        private string _selectedItem;  // selected Item from comboBoxList

        private string _interfaceType;
        private string _name;
        private string _domen;
        private string _speed;
        private string _discription;
        private string _dnsServers;

        private string _bytesReceived;
        private string _packetsReceived;
        private string _bytesSent;
        private string _packetsSent;

        private string _infoPanel;

        private ICommand _buttonStop;
        private ICommand _buttonStart;

        private ICommand _clear;
        private ICommand _clearTabel;
        private ICommand _saveInformation;
        private ICommand _loadInformation;

        private ICommand _sortProtocol;
        private ICommand _sortSourceAdress;
        private ICommand _sortRemoteAdress;
        private ICommand _sortMessageLength;
        private ICommand _sortStandart;

        private sort CurentSort;

        public ViewModel()
        {
            init();
            ListViewBoxUpdate();
            InfoPanel += String.Format("{0} : Program Start \n", DateTime.Now);
        }
        /// <summary>
        /// Initialize work of Taimers
        /// </summary>
        private void initTaimer()
        {
            this.timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(100000);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }
        /// <summary>
        /// Initialize work of Taimers
        /// </summary>
        private void initTaimerForChart()
        {
            this.timerForChart = new DispatcherTimer();
            timerForChart.Interval = new TimeSpan(0, 0, 3);
            timerForChart.Tick += new EventHandler(timer_Tick_Char);
            timerForChart.Start();
        }


        /// <summary>
        /// Initialize base configuration
        /// </summary>
        private void init()
        {
            CurentSort = sort.NON;
            adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapterT in adapters)
            {
                _comboBoxList.Add(adapterT.Name);
            }
            InterfaceType = "Non";
            Name = "Non";
            Domen = "Non";
            Speed = "Non";
            Discription = "Non";
            InfoPanel += String.Format("{0} : Initialization finished \n", DateTime.Now);

        }//init
        /// <summary>
        /// Initialize description of Network Interface adapter
        /// </summary>
        private void IniDiscription()
        {
            try
            {
                for (int i = 0; i < adapters.Count(); i++)
                {
                    if (adapters[i].Name == SelectedItem) { adapter = adapters[i]; }
                }

                IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                IPAddressCollection dnsServers = adapterProperties.DnsAddresses;
                var dnsDexcription = adapterProperties.DnsSuffix;
                Discription = adapter.Description;
                InterfaceType = adapter.NetworkInterfaceType.ToString();
                Domen = dnsDexcription.ToString();
                Speed = ((adapter.Speed / 1000000).ToString() + " Mbps");
                var statistic = adapter.GetIPStatistics();
                DNSservers = "";
                if (dnsServers.Count > 0)
                {
                    foreach (IPAddress dns in dnsServers)
                    {
                        DNSservers += dns.ToString() + "\n";
                    }
                }
            }
            catch (PlatformNotSupportedException e) { InfoPanel += String.Format("{0} : Error : {1} \n", DateTime.Now, e.Message); }
            catch (ArgumentNullException e) { InfoPanel += String.Format("{0} : Error : {1} \n", DateTime.Now, e.Message); }

        }//IniDiscription
        /// <summary>
        /// Collect information of protocols
        /// </summary>
        private void ListViewBoxUpdate()
        {
            try
            {
                var IPv4Addresses = Dns.GetHostEntry(Dns.GetHostName())
                   .AddressList.Where(al => al.AddressFamily == AddressFamily.InterNetwork)
                   .AsEnumerable();
                foreach (IPAddress ip in IPv4Addresses)
                    Sniff(ip);
            }
            catch (ArgumentNullException ee) { InfoPanel += String.Format("{0} : Error : {1} \n", DateTime.Now, ee.Message); }
            catch (SocketException ee) { InfoPanel += String.Format("{0} : Error : {1} \n", DateTime.Now, ee.Message); }
        }
        public static void Sniff(IPAddress ip)
        {
            //setup the socket to listen on, we are listening just to IPv4 IPAddresses
            Socket sck = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
            sck.Bind(new IPEndPoint(ip, 0));
            sck.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);
            sck.IOControl(IOControlCode.ReceiveAll, new byte[4] { 1, 0, 0, 0 }, null);

            //byte array to hold the packet data we want to examine.
            //  we are assuming default (20byte) IP header size + 4 bytes for TCP header to get ports
            byte[] buffer = new byte[sck.ReceiveBufferSize];
            int count = sck.Receive(buffer);
            IPAddreData newConnect;

            // Async methods for recieving and processing data
            Action<IAsyncResult> OnReceive = null;
            OnReceive = (ar) =>
            {
                try
                {
                    newConnect = new IPAddreData(buffer, count);
                    newConnect.PortIn = ((ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, 20))).ToString(); //port in
                    newConnect.PortOut = ((ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(buffer, 22))).ToString(); //pot out
                    if (statusOfWork == true)
                    {
                        _listViewBoxBuffer.Add(newConnect);
                        buffer = new byte[65550]; //clean out our buffer
                        sck.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), null); //listen some more
                    }
                }
                catch (Exception e)
                {
                    string caption = "Ups! We have a Problem!";
                    string message = "ArgumentOutOfRangeException : " + e.Message;
                    DialogResult result = System.Windows.Forms.MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            // begin listening to the socket
            sck.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceive), null);
        }
        /// <summary>
        /// update information in Chart by timer interrupt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick_Char(object sender, EventArgs e)
        {
            if (LoadData.Count > 3)
                LoadData.Clear();
            if (SendData.Count > 3)
                SendData.Clear();
            IPv4InterfaceStatistics interfaceStatistic = adapter.GetIPv4Statistics();
            LoadData.Add(new KeyValuePair<string, long>(DateTime.Now.ToString().Replace(':', ' '), interfaceStatistic.BytesReceived));
            SendData.Add(new KeyValuePair<string, long>(DateTime.Now.ToString().Replace(':', ' '), interfaceStatistic.BytesSent));
        }
        /// <summary>
        /// update information in Table by timer interrupt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                IPv4InterfaceStatistics interfaceStatistic = adapter.GetIPv4Statistics();
                BytesReceived = interfaceStatistic.BytesReceived.ToString();
                BytesSent = interfaceStatistic.BytesSent.ToString();
                PacketsReceived = interfaceStatistic.UnicastPacketsReceived.ToString();
                PacketsSent = interfaceStatistic.UnicastPacketsSent.ToString();

                if (statusOfWork == true)
                {
                    if (CurentSort == sort.NON)
                    {
                        ListViewBox = new ObservableCollection<IPAddreData>(_listViewBoxBuffer);
                    }
                    else if (CurentSort == sort.Protocol)
                    {
                        ListViewBox = new ObservableCollection<IPAddreData>(_listViewBoxBuffer.OrderBy(x => x.Protocol));
                    }
                    else if (CurentSort == sort.SourceAdress)
                    {
                        ListViewBox = new ObservableCollection<IPAddreData>(_listViewBoxBuffer.OrderBy(x => x.SourceAddress.ToString()));
                    }
                    else if (CurentSort == sort.RemoteAdress)
                    {
                        ListViewBox = new ObservableCollection<IPAddreData>(_listViewBoxBuffer.OrderBy(x => x.DestinationAddress.ToString()));
                    }
                    else if (CurentSort == sort.MessageLength)
                    {
                        ListViewBox = new ObservableCollection<IPAddreData>(_listViewBoxBuffer.OrderBy(x => x.MessageLength));
                    }
                }
            }
            catch (ArgumentNullException ee) { InfoPanel += String.Format("{0} : Error : {1} \n", DateTime.Now, ee.Message); }
            catch (InvalidOperationException ee) { InfoPanel += String.Format("{0} : Error : {1} \n", DateTime.Now, ee.Message); }
            
        }//timer_Tick
        public ICommand SortStandart
        {
            get
            {
                return _sortStandart ?? (_sortStandart = new RelayCommand(
                   () => /*Lyambda*/
                   {
                       CurentSort = sort.NON;
                       InfoPanel += String.Format("{0} : Tabel is not sort \n", DateTime.Now);

                   }
                    ));
            }
        }//SortStandart
        public ICommand SortMessageLength
        {
            get
            {
                return _sortMessageLength ?? (_sortMessageLength = new RelayCommand(
                   () => /*Lyambda*/
                   {
                       CurentSort = sort.MessageLength;
                       InfoPanel += String.Format("{0} : Tabel is Sort By SortMessageLength \n", DateTime.Now);
                       if (statusOfWork == false)
                       {
                           ListViewBox = new ObservableCollection<IPAddreData>(_listViewBox.OrderBy(x => x.MessageLength));
                       }
                   }
                    ));
            }
        }//SortMessageLength
        public ICommand SortRemoteAdress
        {
            get
            {
                return _sortRemoteAdress ?? (_sortRemoteAdress = new RelayCommand(
                   () => /*Lyambda*/
                   {
                       CurentSort = sort.RemoteAdress;
                       InfoPanel += String.Format("{0} : Tabel is Sort By SortRemoteAdress \n", DateTime.Now);
                       if (statusOfWork == false)
                       {
                           ListViewBox = new ObservableCollection<IPAddreData>(_listViewBox.OrderBy(x => x.DestinationAddress.ToString()));
                       }
                   }
                    ));
            }
        }//SortRemoteAdress
        public ICommand SortSourceAdress
        {
            get
            {
                return _sortSourceAdress ?? (_sortSourceAdress = new RelayCommand(
                   () => /*Lyambda*/
                   {
                       CurentSort = sort.SourceAdress;
                       InfoPanel += String.Format("{0} : Tabel is Sort By SortLocalAdress \n", DateTime.Now);
                       if (statusOfWork == false)
                       {
                           ListViewBox = new ObservableCollection<IPAddreData>(_listViewBox.OrderBy(x => x.SourceAddress.ToString()));
                       }
                   }
                    ));
            }
        }//SortLocalAdress
        public ICommand SortProtocol
        {
            get
            {
                return _sortProtocol ?? (_sortProtocol = new RelayCommand(
                   () => /*Lyambda*/
                   {
                       CurentSort = sort.Protocol;
                       InfoPanel += String.Format("{0} : Tabel is Sort By Protocol \n", DateTime.Now);
                       if (statusOfWork == false)
                       {
                           ListViewBox = new ObservableCollection<IPAddreData>(_listViewBox.OrderBy(x => x.Protocol));
                       }
                   }
                    ));
            }
        }//SortByProtocol
        public ICommand ButtonStop
        {
            get
            {
                return _buttonStop ?? (_buttonStop = new RelayCommand(
                   () => /*Lyambda*/
                   {
                       //SameLogik 
                       statusOfWork = false;
                       InfoPanel += String.Format("{0} : Tabel uodate is stoped \n", DateTime.Now);
                   }
                    ));
            }
        }/*ButtonStop*/
        public ICommand ButtonStart
        {
            get
            {
                return _buttonStart ?? (_buttonStart = new RelayCommand(
                   () => /*Lyambda*/
                   {
                       //SameLogik 

                       statusOfWork = true;
                       ListViewBoxUpdate();
                       InfoPanel += String.Format("{0} : Tabel is update \n", DateTime.Now);
                   }
                    ));
            }
        }//ButtonClear
        public ICommand Clear
        {
            get
            {
                return _clear ?? (_clear = new RelayCommand(
                   () => /*Lyambda*/
                   {
                       //SameLogik 
                       InfoPanel = " ";
                       InfoPanel += String.Format("{0} : Panel has been cleared \n", DateTime.Now);
                   }
                    ));
            }
        }/*ButtonClear*/
        public ICommand ClearTabel
        {
            get
            {
                return _clearTabel ?? (_clearTabel = new RelayCommand(
                   () => /*Lyambda*/
                   {
                       //SameLogik 
                       ListViewBox = new ObservableCollection<IPAddreData>();
                       _listViewBoxBuffer = new List<IPAddreData>();
                       InfoPanel += String.Format("{0} : Tabel has been cleared \n", DateTime.Now);
                   }
                    ));
            }
        }/*ClearTabel*/
        public ICommand SaveInformation
        {
            get
            {
                return _saveInformation ?? (_saveInformation = new RelayCommand(
                   () => /*Lyambda*/
                   {
                       //SameLogik 
                       _session.ListIPBoxBuffer = _listViewBoxBuffer;
                       _session.Name = "CurrentInfo" + DateTime.Now.ToString().Replace(':', '-');
                       var thread = new System.Threading.Thread(new System.Threading.ThreadStart(DisplayFormThread));
                       thread.SetApartmentState(ApartmentState.STA);
                       thread.IsBackground = true;
                       thread.Start();
                       InfoPanel += String.Format("{0} : Information in save mode \n", DateTime.Now);
                   }
                    ));
            }
        }/*SaveInformation*/
        private void DisplayFormThread()
        {
            try
            {
                ViewSaveWindow saveWindow = new ViewSaveWindow();
                saveWindow.Show();
                saveWindow.Closed += (s, e) => System.Windows.Threading.Dispatcher.ExitAllFrames();
                System.Windows.Threading.Dispatcher.Run();
            }
            catch (Exception ex)
            {
                InfoPanel += String.Format("{0} : Error : {1} \n", DateTime.Now, ex.Message);
            }
        }
        public ICommand LoadInformation
        {
            get
            {
                return _loadInformation ?? (_loadInformation = new RelayCommand(
                   () => /*Lyambda*/
                   {
                       //SameLogik 
                       var thread = new System.Threading.Thread(new System.Threading.ThreadStart(DisplayFormThreadLoad));
                       thread.SetApartmentState(ApartmentState.STA);
                       // thread.IsBackground = true;
                       thread.Start();
                       InfoPanel += String.Format("{0} : Information in save mode \n", DateTime.Now);
                   }
                    ));
            }
        }/*SaveInformation*/
        private void DisplayFormThreadLoad()
        {
            try
            {
                ViewLoaderWindow loadWindow = new ViewLoaderWindow();
                loadWindow.Show();
                loadWindow.Closed += (s, e) => System.Windows.Threading.Dispatcher.ExitAllFrames();
                System.Windows.Threading.Dispatcher.Run();
            }
            catch (Exception ex)
            {
                InfoPanel += String.Format("{0} : Error : {1} \n", DateTime.Now, ex.Message);
            }
        }
        public String BytesReceived
        {
            get { return _bytesReceived; }
            set { _bytesReceived = value; RaisePropertyChanged(() => BytesReceived); }
        }
        public String PacketsReceived
        {
            get { return _packetsReceived; }
            set { _packetsReceived = value; RaisePropertyChanged(() => PacketsReceived); }
        }
        public String BytesSent
        {
            get { return _bytesSent; }
            set { _bytesSent = value; RaisePropertyChanged(() => BytesSent); }
        }
        public String PacketsSent
        {
            get { return _packetsSent; }
            set { _packetsSent = value; RaisePropertyChanged(() => PacketsSent); }
        }
        public String InterfaceType
        {
            get { return _interfaceType; }
            set { _interfaceType = value; RaisePropertyChanged(() => InterfaceType); }
        }
        public String Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(() => Name); }

        }
        public String Domen
        {
            get { return _domen; }
            set { _domen = value; RaisePropertyChanged(() => Domen); }

        }
        public String Speed
        {
            get { return _speed; }
            set { _speed = value; RaisePropertyChanged(() => Speed); }

        }
        public String Discription
        {
            get { return _discription; }
            set { _discription = value; RaisePropertyChanged(() => Discription); }

        }
        public String DNSservers
        {
            get { return _dnsServers; }
            set { _dnsServers = value; RaisePropertyChanged(() => DNSservers); }

        }
        public ObservableCollection<IPAddreData> ListViewBox
        {

            get { lock (lockThis) { return _listViewBox; } }
            set { lock (lockThis) { _listViewBox = value; RaisePropertyChanged(() => ListViewBox); } }
        }
        public ObservableCollection<KeyValuePair<string, long>> LoadData
        {
            get { lock (lockThis) { return _loadData; } }
            set { lock (lockThis) { _loadData = value; RaisePropertyChanged(() => LoadData); } }
        }
        public ObservableCollection<KeyValuePair<string, long>> SendData
        {

            get { lock (lockThis) { return _sendData; } }
            set { lock (lockThis) { _sendData = value; RaisePropertyChanged(() => SendData); } }
        }

        public ObservableCollection<String> ComboBoxList
        {
            get { return _comboBoxList; }
            set { _comboBoxList = value; RaisePropertyChanged(() => ComboBoxList); }
        }
        public String SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; RaisePropertyChanged(() => SelectedItem); Name = SelectedItem; IniDiscription(); initTaimer(); initTaimerForChart();/*statusOfWork = true;*/ InfoPanel += String.Format("{0} : Selected Interfase - {1} \n", DateTime.Now, Name); }

        }
        public String InfoPanel
        {
            get { return _infoPanel; }
            set { _infoPanel = value; RaisePropertyChanged(() => InfoPanel); }

        }


    }
}
