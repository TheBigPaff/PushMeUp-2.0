using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushMeUp_2._0_by_Deniso
{
    public class UserSettings
    {
        public int Id { get; set; }
        public int IgnoreSelfRating { get; set; }
        public int EmailReport { get; set; }
        public int StartupPageId { get; set; }



        public string StartupPage { 
            get 
            {
                return SqliteDataAccess.GetStartupPageString(StartupPageId);
            }
            set
            {
                StartupPageId = SqliteDataAccess.GetIdFromStartupPage(value);
            }
        }

        public bool IgnoreSelfRatingBool
        {
            get
            {
                return IgnoreSelfRating == 1;
            }
            set
            {
                IgnoreSelfRating = value ? 1 : 0;
            }
        }
        public bool EmailReportBool
        {
            get
            {
                return EmailReport == 1;
            }
            set
            {
                EmailReport = value ? 1 : 0;
            }
        }
    }
}
