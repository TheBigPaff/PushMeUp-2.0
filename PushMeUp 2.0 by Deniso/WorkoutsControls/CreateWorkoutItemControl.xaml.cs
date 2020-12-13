using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PushMeUp_2._0_by_Deniso
{
    /// <summary>
    /// Interaction logic for CreateWorkoutItemControl.xaml
    /// </summary>
    public partial class CreateWorkoutItemControl : UserControl
    {
        Workout workout;
        Exercise exercise;
        ExerciseInfoControl parent;

        public CreateWorkoutItemControl(Workout _workout, Exercise _exercise, ExerciseInfoControl _parent)
        {
            InitializeComponent();

            workout = _workout;
            exercise = _exercise;
            parent = _parent;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            workoutItemForm.Exercise = exercise;
            workoutItemForm.Workout = workout;
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
            if (contentArea != null)
            {
                contentArea.Content = parent;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (workoutItemForm.AddWorkoutItemToDB())
            {
                MessageBox.Show($"Exercise added to {workout.Name} succesfully!");
                ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
                if (contentArea != null)
                {
                    contentArea.Content = new WorkoutInfoControl(workout);
                }
            }
        }
    }
}
