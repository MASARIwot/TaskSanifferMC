using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SnifferMC.Resourse;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace SnifferMC
{
    public class ViewLoaderModel : ViewModelBase
    {
        private ObservableCollection<IPAddreData> _listViewBox = new ObservableCollection<IPAddreData>();
        private SessionLoad _sessionLoad = SessionLoad.Instance;
        private SessionSave _sessionSave = SessionSave.Instance;

        private sort CurentSort;
        private ICommand _sortProtocol;
        private ICommand _sortSourceAdress;
        private ICommand _sortRemoteAdress;
        private ICommand _sortMessageLength;
        private ICommand _sortStandart;

        private ICommand _clearTabel;
        private ICommand _save;
        private ICommand _saveExcel;
        private ICommand _load;
        


        public ViewLoaderModel()
        {

        }//ViewLoaderModel
        /// <summary>
        /// Load information and save in SessionLoad.class->"SessionLoad.ListIPBoxBuffer=" 
        /// Filter is XML Files
        /// </summary>
        /// <see cref="SessionLoad"/>
        private void loader()
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.InitialDirectory = "c:\\";
            choofdlog.Filter = "XML Files (*.xml)|*.xml";
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = false;

            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string sFileName = choofdlog.FileName;
                    _sessionLoad.ListIPBoxBuffer = FileWorker.xmlDeserializer(sFileName);
                    ListViewBox = new ObservableCollection<IPAddreData>(_sessionLoad.ListIPBoxBuffer);
                }
                catch (NotSupportedException e) { problems0_o(e); }
                catch (DirectoryNotFoundException e) { problems0_o(e); }
                catch (FileNotFoundException e) { problems0_o(e); }
                catch (ArgumentNullException e) { problems0_o(e); }
                catch (InvalidOperationException e) { problems0_o(e); }
            }
        }
        private void SaveExel()
        {
            //.xls 
            if (_sessionLoad.ListIPBoxBuffer != null)
            {
                SaveFileDialog choofdlog = new SaveFileDialog();
                choofdlog.InitialDirectory = "c:\\";
                choofdlog.Filter = "xls Files (*.xls)|*.xls";
                choofdlog.FilterIndex = 1;
                //choofdlog.Multiselect = false;

                if (choofdlog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string sFileName = choofdlog.FileName ;
                        new FileExcelWorker(_sessionLoad.ListIPBoxBuffer, sFileName).SaveToExel();
                    }
                    catch (NotSupportedException e) { problems0_o(e); }
                    catch (DirectoryNotFoundException e) { problems0_o(e); }
                    catch (FileNotFoundException e) { problems0_o(e); }
                    catch (ArgumentNullException e) { problems0_o(e); }
                    catch (InvalidOperationException e) { problems0_o(e); }
                }
            }
        }
        /// <summary>
        /// Processing Exceptions
        /// </summary>
        /// <param name="e"></param>
        private void problems0_o(Exception e)
        {
            string caption = "We have a Problem 0_o?";
            string message = e.Message;
            DialogResult result = System.Windows.Forms.MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public ICommand Load
        {
            get
            {
                return _load ?? (_load = new RelayCommand(
                   () => /*Lyambda*/
                   {
                       loader();
                   }
                    ));
            }
        }/*Load*/
        public ICommand Save
        {
            get
            {
                return _save ?? (_save = new RelayCommand(
                   () => /*Lyambda*/
                   {
                       _sessionSave.ListIPBoxBuffer = _sessionLoad.ListIPBoxBuffer;
                       _sessionSave.Name = "loadInfo" + DateTime.Now.ToString().Replace(':', '-');
                       var thread = new System.Threading.Thread(new System.Threading.ThreadStart(DisplayFormThread));
                       thread.SetApartmentState(ApartmentState.STA);
                       thread.IsBackground = true;
                       thread.Start();
                   }
                    ));
            }
        }/*SaveInformation*/

        public ICommand SaveExcel
        {
            get
            {
                return _saveExcel ?? (_saveExcel = new RelayCommand(
                   () => /*Lyambda*/
                   {
                       SaveExel();
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
            catch (Exception e)
            {
                problems0_o(e);
            }
        }//DisplayFormThread
        public ICommand ClearTabel
        {
            get
            {
                return _clearTabel ?? (_clearTabel = new RelayCommand(
                   () => /*Lyambda*/
                   {
                       ListViewBox = new ObservableCollection<IPAddreData>();
                       _sessionLoad.ListIPBoxBuffer = new List<IPAddreData>();
                   }
                    ));
            }
        }/*ClearTabel*/
        public ObservableCollection<IPAddreData> ListViewBox
        {
            get { return _listViewBox; }
            set { _listViewBox = value; RaisePropertyChanged(() => ListViewBox); }
        }
        public ICommand SortStandart
        {
            get
            {
                return _sortStandart ?? (_sortStandart = new RelayCommand(
                   () => /*Lyambda*/
                   {
                       CurentSort = sort.NON;
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
                       ListViewBox = new ObservableCollection<IPAddreData>(_listViewBox.OrderBy(x => x.MessageLength));
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
                       ListViewBox = new ObservableCollection<IPAddreData>(_listViewBox.OrderBy(x => x.DestinationAddress.ToString()));
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
                       ListViewBox = new ObservableCollection<IPAddreData>(_listViewBox.OrderBy(x => x.SourceAddress.ToString()));
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
                       ListViewBox = new ObservableCollection<IPAddreData>(_listViewBox.OrderBy(x => x.Protocol));
                   }
                    ));
            }
        }//SortByProtocol


    }//ViewLoaderModel
}
