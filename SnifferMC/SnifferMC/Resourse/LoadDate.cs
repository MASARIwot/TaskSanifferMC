using SnifferMC.Data;
using SnifferMC.Data.Resourses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnifferMC.Resourse
{
    public class LoadDate
    {
        public DateTime Time { get; set; }
        public double Value { get; set; }
        public LoadDate(DateTime time,double vale) 
        {
            Time = time;
            Value = vale;
                       
        }
    }
}
