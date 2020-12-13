using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushMeUp_2._0_by_Deniso
{
    public static class Session
    {
        public static User LoggedUser { get; set; }
        public static UserSettings Settings { get; set; }
    }
}
