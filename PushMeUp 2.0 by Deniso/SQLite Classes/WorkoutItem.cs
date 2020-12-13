using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushMeUp_2._0_by_Deniso
{
    public class WorkoutItem
    {
        public int Id { get; set; }

        public int Duration { get; set; } 
        public double Distance { get; set; } 
        public int Reps { get; set; } 
        //public int Sets { get; set; }

        public string EquipmentUsed { get; set; } // "25kg dumbbells", "10kg some weird weight no one has ever heard of", "2lbs jump rope"

        public string ItemType { get; set; } // "Break", "Distance", "RepsSets", "Duration"
        public Exercise Exercise { get; set; }


        public WorkoutItem()
        {
            
        }
        public WorkoutItem(string _itemType, int _duration) // for breaks
        {
            ItemType = _itemType;
            Duration = _duration;
        }
        public WorkoutItem(string _itemType, Exercise _exercise) // for To Failure
        {
            ItemType = _itemType;
            Exercise = _exercise;
        }
        public WorkoutItem(string _itemType, int _duration, Exercise _exercise)
        {
            ItemType = _itemType;
            Duration = _duration;
            Exercise = _exercise;
        }
        public WorkoutItem(string _itemType, double _distance, Exercise _exercise)
        {
            ItemType = _itemType;
            Distance = _distance;
            Exercise = _exercise;
        }
        public WorkoutItem(int _reps, string _itemType, Exercise _exercise)
        {
            ItemType = _itemType;
            Reps = _reps;
            Exercise = _exercise;
        }
    }
}
