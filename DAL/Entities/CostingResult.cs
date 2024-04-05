using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
   public class CostingResult
    {
        public decimal ComponentCosting{ get; set; }
        public decimal FormulationCosting{ get; set; }
        public decimal TotalCosting{ get; set; }
    }
}
