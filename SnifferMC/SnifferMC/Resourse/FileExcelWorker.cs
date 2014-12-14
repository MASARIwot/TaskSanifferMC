using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace SnifferMC.Resourse
{
    public sealed class FileExcelWorker
    {
        private List<IPAddreData> _listIPBoxBuffer;
        private string _path;
        private Worksheet _workShee;
        private int rowExcel = 2; //начать со второй строки.
        private Excel.Application exApp;
        public FileExcelWorker(List<IPAddreData> listIPBoxBuffer, string path) 
        {
            this._listIPBoxBuffer = listIPBoxBuffer;
            this._path = path;
            init();

        }
        public FileExcelWorker(List<IPAddreData> listIPBoxBuffer)
        {
            this._listIPBoxBuffer = listIPBoxBuffer;
            this._path = Environment.CurrentDirectory + "\\" + "MyFile.xls";
            
        }
        public FileExcelWorker()
        {
        }
        public void SaveToExel()
        {
            init();
            makeRows();
            Save();

        }
        private void init() 
        {
            exApp = new Excel.Application();
            exApp.Visible = true; // для отладки(данную строку можно не указывать)
            exApp.UserControl = true; 
            exApp.Workbooks.Add();
            this._workShee = (Worksheet)exApp.ActiveSheet;

            _workShee.Cells[1, 1] = "Protocol";
            _workShee.Cells[1, "B"] = "PotIn";
            _workShee.Cells[1, 3] = "SourceAdress";
            _workShee.Cells[1, 4] = "Remote Adress";
            _workShee.Cells[1, 5] = "PortOut";
            _workShee.Cells[1, 6] = "MessageLength";
        }
        private void makeRows() 
        {

            for (int i = 0; i < _listIPBoxBuffer.Count; i++)
            {
                //заполняем строку
                _workShee.Cells[rowExcel, "A"] = _listIPBoxBuffer[i].Protocol.ToString();
                _workShee.Cells[rowExcel, "B"] = _listIPBoxBuffer[i].PortIn.ToString();
                _workShee.Cells[rowExcel, "C"] = _listIPBoxBuffer[i].SourceAddress.ToString();
                _workShee.Cells[rowExcel, "D"] = _listIPBoxBuffer[i].DestinationAddress.ToString();
                _workShee.Cells[rowExcel, "E"] = _listIPBoxBuffer[i].PortOut.ToString();
                _workShee.Cells[rowExcel, "F"] = _listIPBoxBuffer[i].MessageLength.ToString();

                ++rowExcel;
            }
        }
        private void Save()
        {
            
            _workShee.SaveAs(this._path);
            exApp.Quit();
        }

    }
}
