using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnifferMC.Resourse
{
   public class SendDate
    {
        public DateTime Time { get; set; }
        public double Value { get; set; }
        public SendDate(DateTime time, double vale) 
        {
            Time = time;
            Value = vale;
        }
    }
}
