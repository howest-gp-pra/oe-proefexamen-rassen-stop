using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProefEx.LIB.Entities
{
    public class Ras
    {
        public int ID { get; set; }
        public string RasNaam { get; set; }

        public override string ToString()
        {
            return RasNaam;
        }
    }
}
