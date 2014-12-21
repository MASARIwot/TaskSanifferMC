using SnifferMC.Resourse;
using System;
using System.Collections.Generic;
namespace SnifferMC.Data.Resourses
{
    public interface IDbProcessing
    {
        /// <summary>
        /// Add date in new row
        /// </summary>
        /// <param name="imba"></param>
        ///<exception cref="InvalidOperationException"></exception>
        void AddDate(IPAddreData imba);
        /// <summary>
        /// Clear table
        /// </summary>
        /// <param name="table"></param>
        /// <exception cref="InvalidOperationException"></exception>
        void ClearDbTable();
        /// <summary>
        /// Clear table *ipadapters_t
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        void ClearDbTable(string table = "ipadapters_t");
        /// <summary>
        /// Dispose
        /// </summary>
        void Dispose();
        /// <summary>
        /// get current ID
        /// </summary>
        int getId { get; set; }
        /// <summary>
        /// Load list of IpAdapters data 
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>list of IpAdapters </returns>
        List<IpAdapters> LoadData();
        /// <summary>
        /// Save date to database
        /// </summary>
        /// <param name="listIPBoxBuffer"></param>
        ///<exception cref="InvalidOperationException"></exception>
        void SaveDate(List<IPAddreData> listIPBoxBuffer);
        /// <summary>
        /// Processing user sql command 
        /// </summary>
        /// <param name="query"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>list of IpAdapters </returns>
        List<IpAdapters> SQLuserCommand(string query);
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
        void UpdateDate(int id, string PortIn, string PortOut, string Protocol, string UiDestinationIPAddress, string UiSourceIPAddress, string MessageLength);
    }
}
