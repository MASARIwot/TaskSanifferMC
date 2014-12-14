using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnifferMC.Resourse
{
    /// <summary>
    /// Collect information for saving in file 
    /// </summary>
    public class SessionSave
    {
        //object for lock method
        private static object syncRoot = new Object();
        //List to Save
        private List<IPAddreData> _listIPBoxBuffer;
        //Name Of File
        public string Name { get; set; }

        #region Instance <Singleton>
        private static volatile SessionSave instance;
        private SessionSave() { }
        public static SessionSave Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SessionSave();
                    }
                }
                return instance;
            }
        }//Instance
        #endregion
        
        public List<IPAddreData> ListIPBoxBuffer
        {
            get { lock (syncRoot) { return this._listIPBoxBuffer; } }
            set { lock (syncRoot) { this._listIPBoxBuffer = value; } }
        }
        /// <summary>
        /// clr list with information
        /// </summary>
        public void clrListIPBoxBuffer()
        {
            lock (syncRoot) { ListIPBoxBuffer = null; }
        }
    }
}
