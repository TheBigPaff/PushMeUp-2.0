using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushMeUp_2._0_by_Deniso
{
    public class Workout
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<WorkoutItem> WorkoutItems { get; set; }
    }
}
