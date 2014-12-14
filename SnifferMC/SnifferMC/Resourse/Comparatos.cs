using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnifferMC.Resourse
{
    /// <summary>
    /// this class contains different comparators for IPAddreData.class
    /// </summary>
   public class Comparatos
    {
       public static int CompareProtocol(IPAddreData one, IPAddreData other)
        {
            return one.Protocol.CompareTo(other.Protocol);
        }
       public static int CompareSourceAdress(IPAddreData one, IPAddreData other)
        {
            return one.SourceAddress.ToString().CompareTo(other.SourceAddress.ToString());
        }
       public static int CompareToPhoneNumber(IPAddreData one, IPAddreData other)
        {
            return one.MessageLength.CompareTo(other.MessageLength);
        }
       public static int CompareToData(IPAddreData one, IPAddreData other)
        {
            return one.DestinationAddress.ToString().CompareTo(other.DestinationAddress.ToString());
        }
    }
}
