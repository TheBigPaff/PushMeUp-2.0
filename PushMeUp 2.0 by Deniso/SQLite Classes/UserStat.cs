using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushMeUp_2._0_by_Deniso
{
    public class UserStat
    {
        public int Id { get; set; }
        public string Day { get; set; }
        public string StartingTime { get; set; }
        public string FinishingTime { get; set; }
        public int SelfRating { get; set; }
        public Workout completedWorkout { get; set; }

        public UserStat()
        {
            completedWorkout = new Workout();
        }
    }
}
