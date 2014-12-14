using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using SnifferMC.Resourse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace SnifferMC
{
    /// <summary>
    /// This class stores the information from the sample
    /// Standart File name : Directory + (@"\InformationLog") + SessionSave->_session.Name + ".xml";
    /// </summary>
    public class ViewSaveModel : ViewModelBase
    {
        
        private SessionSave _session = SessionSave.Instance;
        private static List<IPAddreData> _listViewBoxBuffer;
        private string _directory;
        private ICommand _buttonAddDir;
        private ICommand _buttonSave;
        private ICommand _buttonExit;
        public ViewSaveModel()
        {
            init();  
        }
        private void init() 
        {

            Directory = Path.Combine(Environment.CurrentDirectory);
        }
        /// <summary>
        /// Save information from sample
        /// </summary>
        private void save() 
        {
            try
            {
                string _path = Directory + (@"\InformationLog") + _session.Name + ".xml";
                Directory = _path;
                _listViewBoxBuffer = _session.ListIPBoxBuffer;
                FileWorker.xmlSerializer(_listViewBoxBuffer, (Directory));
                Directory = "Iformation saved";
            }
            catch (NotSupportedException e)
            {
                string caption = "We have a Problem 0_o?";
                string message = "NotSupportedException : " + e.Message;
                DialogResult result = System.Windows.Forms.MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (DirectoryNotFoundException e)
            {
                string caption = "Ups! We have a Problem!";
                string message = "DirectoryNotFoundException : " + e.Message;
                DialogResult result = System.Windows.Forms.MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
                     
        }
        /// <summary>
        /// Get or Set Directory for file location
        /// </summary>
        public String Directory
        {
            get { return _directory; }
            set { _directory = value; RaisePropertyChanged(() => Directory); }

        }
        public ICommand ButtonExit
        {
            get
            {
                return _buttonExit ?? (_buttonExit = new RelayCommand(

                   () => /*Lyambda*/
                   {
                      // System.Windows.Application.Current.MainWindow.Close();
                   }
                    ));
            }
        }//ButtonExit
        public ICommand ButtonSave
        {
            get
            {
                return _buttonSave ?? (_buttonSave = new RelayCommand(
                   () => /*Lyambda*/
                   {
                       save();
                   }
                    ));
            }
        }//ButtonSave
        public ICommand ButtonAddDir
        {
            get
            {
                return _buttonAddDir ?? (_buttonAddDir = new RelayCommand(
                   () => /*Lyambda*/
                   {
                       FolderBrowserDialog profilePath = new FolderBrowserDialog();

                       if (profilePath.ShowDialog() == DialogResult.OK)
                       {
                           Directory = profilePath.SelectedPath;
                       }
                       else { Directory = Path.Combine(Environment.CurrentDirectory); }
                   }
                    ));
            }
        }//ButtonAddDir
        /// <summary>
        /// Location of the Image for window
        /// </summary>
        public string DisplayedImagePath
        {
            get 
            {
                string path = Path.Combine(Environment.CurrentDirectory, @"Image\WindowSaveIcon.jpg");
                return path; 
            }
        }//DisplayedImagePath
    }
}
