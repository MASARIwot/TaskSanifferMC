using SnifferMC.Data.Resourses;
using SnifferMC.Resourse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnifferMC.Data
{
    /// <summary>
    /// Operation with database
    /// </summary>
    public class DbProcessing : IDisposable, IDbProcessing
    {
        /// <summary>
        /// Db Model 
        /// </summary>
        private ipadapterContext context;
        /// <summary>
        /// Id counter
        /// </summary>
        private static int _idCounter = 0;

        public void SaveDateDb()
        {
            this.context = new ipadapterContext();
        }//SaveDateDb.class
        /// <summary>
        /// Save date to database
        /// </summary>
        /// <param name="listIPBoxBuffer"></param>
        ///<exception cref="InvalidOperationException"></exception>
        public void SaveDate(List<IPAddreData> listIPBoxBuffer)
        {
            try
            {
                for (int i = 0; i < listIPBoxBuffer.Count; i++)
                {

                    var imba = listIPBoxBuffer[i];
                    context.IpAdapters.Add(new IpAdapters()
                    {
                        Id = getId,
                        MessageLength = imba.MessageLength.ToString(),
                        PortIn = imba.PortIn.ToString(),
                        PortOut = imba.PortOut.ToString(),
                        Protocol = imba.Protocol.ToString(),
                        UiDestinationIPAddress = imba.DestinationAddress.ToString(),
                        UiSourceIPAddress = imba.SourceAddress.ToString()
                    });
                    context.SaveChanges();
                }//for
               
            }
            catch (InvalidOperationException e) { throw new InvalidOperationException(e.Message); }
        }//SaveDate
        /// <summary>
        /// Add date in new row
        /// </summary>
        /// <param name="imba"></param>
        ///<exception cref="InvalidOperationException"></exception>
        public void AddDate(IPAddreData imba)
        {
            try
            {
                context.IpAdapters.Add(new IpAdapters()
                {
                    Id = getId,
                    MessageLength = imba.MessageLength.ToString(),
                    PortIn = imba.PortIn.ToString(),
                    PortOut = imba.PortOut.ToString(),
                    Protocol = imba.Protocol.ToString(),
                    UiDestinationIPAddress = imba.DestinationAddress.ToString(),
                    UiSourceIPAddress = imba.SourceAddress.ToString()
                });
                context.SaveChanges();
            }
            catch (InvalidOperationException e) { throw new InvalidOperationException(e.Message); }
        }//AddDate
        /// <summary>
        /// Clear table
        /// </summary>
        /// <param name="table"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void ClearDbTable(string table = "ipadapters_t")
        {
            try
            {
                context.Database.ExecuteSqlCommand(@"TRUNCATE TABLE" + table + " ");
                context.SaveChanges();
            }
            catch (InvalidOperationException e) { throw new InvalidOperationException(e.Message); }
        }//ClearDbTable
        /// <summary>
        /// Clear table *ipadapters_t
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void ClearDbTable()
        {
            try
            {
                context.Database.ExecuteSqlCommand("TRUNCATE TABLE ipadapters_t");
                context.SaveChanges();
            }
            catch (InvalidOperationException e) { throw new InvalidOperationException(e.Message); }
            //catch (Exception e) { throw new Exception(e.Message); }
        }//ClearDbTable
        /// <summary>
        /// Load list of IpAdapters data 
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>list of IpAdapters </returns>
        public List<IpAdapters> LoadData()
        {
            try
            {
                IQueryable<IpAdapters> query = context.IpAdapters.AsNoTracking();
                List<IpAdapters> res = query.ToList();
                return res;
            }
            catch (ArgumentNullException e) { throw new ArgumentNullException(e.Message); }
            catch (InvalidOperationException e) { throw new InvalidOperationException(e.Message); }
        }//LoadData
        /// <summary>
        /// Update Date Table By Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="PortIn"></param>
        /// <param name="PortOut"></param>
        /// <param name="Protocol"></param>
        /// <param name="UiDestinationIPAddress"></param>
        /// <param name="UiSourceIPAddress"></param>
        /// <param name="MessageLength"></param>
        ///  <exception cref="ArgumentNullException"></exception>
        ///  <exception cref="InvalidOperationException"></exception>
        public void UpdateDate(int id, string PortIn, string PortOut, string Protocol, string UiDestinationIPAddress, string UiSourceIPAddress, string MessageLength)
        {
            try
            {
                var SameChahjed = context.IpAdapters.Single(c => c.Id == id);
                SameChahjed.MessageLength = MessageLength;
                SameChahjed.PortIn = PortIn;
                SameChahjed.PortOut = PortOut;
                SameChahjed.Protocol = Protocol;
                SameChahjed.UiDestinationIPAddress = UiDestinationIPAddress;
                SameChahjed.UiSourceIPAddress = UiSourceIPAddress;
                context.SaveChanges();
            }
            catch (ArgumentNullException e) { throw new ArgumentNullException(e.Message); }
            catch (InvalidOperationException e) { throw new InvalidOperationException(e.Message); }
        }
        /// <summary>
        /// Processing user sql command 
        /// </summary>
        /// <param name="query"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>list of IpAdapters </returns>
        public List<IpAdapters> SQLuserCommand(string query)
        {
            try
            {
                List<IpAdapters> result = context.IpAdapters.SqlQuery(query).ToList<IpAdapters>();
                return result;
            }
            catch (ArgumentNullException e) { throw new ArgumentNullException(e.Message); }
            catch (InvalidOperationException e) { throw new InvalidOperationException(e.Message); }
        }//SQLuserCommand

        public void Dispose()
        {
            if (context != null)
                context.Dispose();
        }//Dispose
        /// <summary>
        /// get current ID
        /// </summary>
        public int getId
        {
            get { _idCounter = getLastId(); return ++_idCounter; }
            set { _idCounter = value; }
        }//getId
        /// <summary>
        /// get Id of last row 
        /// </summary>
        /// <param name="query"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        private int getLastId(string query = @"SELECT * FROM ipadapters_t ORDER BY Id DESC LIMIT 1")
        {
            try
            {
                var result = context.IpAdapters.SqlQuery(query).AsNoTracking().ToList();
                if (result.Count > 0)
                    return result[0].Id;
                else
                    return 0;
            }
            catch (ArgumentNullException e) { throw new ArgumentNullException(e.Message); }
            catch (InvalidOperationException e) { throw new InvalidOperationException(e.Message); }
        }//getLastId
    }
}
