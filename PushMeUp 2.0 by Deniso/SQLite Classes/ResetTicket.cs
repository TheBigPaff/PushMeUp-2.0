using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushMeUp_2._0_by_Deniso.SQLite_Classes
{
    class ResetTicket
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string ExpirationDate { get; set; }
        public int TokenUsed { get; set; } // basically a bool, 0 = false, 1 = true
    }
}
