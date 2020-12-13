using PushMeUp_2._0_by_Deniso.CControls;
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
    /// Interaction logic for WorkoutsControl.xaml
    /// </summary>
    public partial class WorkoutsControl : UserControl
    {
        List<Workout> workouts;
        public WorkoutsControl()
        {
            InitializeComponent();


            workouts = SqliteDataAccess.LoadWorkouts();
            LoadCardControls();
        }

        private void LoadCardControls()
        {
            if (workouts.Count > 0)
            {
                noWorkoutsGrid.Visibility = Visibility.Collapsed;

                foreach (Workout _workout in workouts)
                {
                    cardsWp.Children.Add(new WorkoutControl
                    {
                        WorkoutId = _workout.Id,
                        WorkoutName = _workout.Name,
                        BreaksCount = "Breaks: " + _workout.WorkoutItems.Where(x => x.ItemType.Equals("Break")).Count().ToString(),
                        ExercisesCount = "Exercises: " + _workout.WorkoutItems.Where(x => !x.ItemType.Equals("Break")).Count().ToString()
                    });
                }
            }
            else
            {
                noWorkoutsGrid.Visibility = Visibility.Visible;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
            if (contentArea != null)
            {
                this.Width = contentArea.Width;
                this.Height = contentArea.Height;
            }
        }

        private void createNewBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
            if (contentArea != null)
            {
                contentArea.Content = new CreateNewWorkoutControl()
                {
                    Width = contentArea.Width,
                    Height = contentArea.Height
                };
            }
        }
        public void WorkoutControlClicked(WorkoutControl _workoutControl)
        {
            ChooseWorkoutControl chooseWorkout;
            if ((chooseWorkout = Helper.GetAncestorOfType<ChooseWorkoutControl>(this)) != null) // check if we're trying to add an exercise to a workout or just trying to show an exercise info
            {
                chooseWorkout.AddToWorkout(workouts.Find(x => x.Id == _workoutControl.WorkoutId));
            }
            else
            {
                workouts = SqliteDataAccess.LoadWorkouts();
                Workout workout = workouts.Find(x => x.Id == _workoutControl.WorkoutId);

                ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
                if (contentArea != null)
                {
                    contentArea.Content = new WorkoutInfoControl(workout);
                }
            }
        }

        private void SearchTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTb.Text)) LoadCardControls();
            if (workouts == null) return;

            cardsWp.Children.Clear();
            foreach (Workout _workout in workouts)
            {
                string searchText = SearchTb.Text.ToLower();

                // search algorithm
                if (_workout.Name.ToLower().Contains(searchText) || _workout.WorkoutItems.Exists(x => x.Exercise != null && x.Exercise.Name.ToLower().Contains(searchText)) 
                    || _workout.WorkoutItems.Exists(x => x.Exercise != null && x.Exercise.TargetedMuscles.Exists(y => y.ToLower().Contains(searchText))) || _workout.WorkoutItems.Exists(x => x.Exercise != null && x.Exercise.ExerciseTypes.Exists(y => y.ToLower().Contains(searchText))))
                {
                    cardsWp.Children.Add(new WorkoutControl
                    {
                        WorkoutId = _workout.Id,
                        WorkoutName = _workout.Name,
                        BreaksCount = "Breaks: " + _workout.WorkoutItems.Where(x => x.ItemType.Equals("Break")).Count().ToString(),
                        ExercisesCount = "Exercises: " + _workout.WorkoutItems.Where(x => !x.ItemType.Equals("Break")).Count().ToString()
                    });
                }
            }
        }
    }
}
