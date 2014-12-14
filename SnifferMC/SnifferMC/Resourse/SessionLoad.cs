using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnifferMC.Resourse
{
    /// <summary>
    /// save Loaded information(List<IPAddreData>) from file  
    /// </summary>
    public class SessionLoad
    { 
        //object for lock method
        private static object syncRoot = new Object();
        //List to save
        private List<IPAddreData> _listIPBoxBuffer;
        

        #region Instance <Singleton>
        private static volatile SessionLoad instance;
        private SessionLoad() { }
        public static SessionLoad Instance
        {
            get{
                if (instance == null){
                    lock (syncRoot){
                        if (instance == null)
                            instance = new SessionLoad();
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
        public void clrListIPBoxBuffer()
        {
            lock (syncRoot) { ListIPBoxBuffer = null; }
        }
    }
}
