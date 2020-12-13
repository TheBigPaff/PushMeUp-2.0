using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushMeUp_2._0_by_Deniso
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public List<string> TargetedMuscles { get; set; }
        public List<string> ExerciseTypes { get; set; }

        public Exercise()
        {
            TargetedMuscles = new List<string>();
            ExerciseTypes = new List<string>();
        }
        public Exercise(string _name, string _description, string _image)
        {
            Name = _name;
            Description = _description;
            Image = _image;

            TargetedMuscles = new List<string>();
            ExerciseTypes = new List<string>();
        }
        public Exercise(int _id, string _name, string _description, string _image)
        {
            Id = _id;
            Name = _name;
            Description = _description;
            Image = _image;

            TargetedMuscles = new List<string>();
            ExerciseTypes = new List<string>();
        }

        public override bool Equals(object obj)
        {
            return (this as Exercise).Id == (obj as Exercise).Id;
        }
    }
}
