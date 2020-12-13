using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
    /// Interaction logic for ExerciseInfoControl.xaml
    /// </summary>
    public partial class ExerciseInfoControl : UserControl
    {
        Exercise exercise;

        public ExerciseInfoControl(Exercise _exercise)
        {
            InitializeComponent();

            exercise = _exercise;
        }

        private void _ExerciseInfoControl_Loaded(object sender, RoutedEventArgs e)
        {
            nameTb.Text = exercise.Name;
            descTb.Text = exercise.Description;
            typesTb.Text = string.Join(", ", exercise.ExerciseTypes);
            musclesTb.Text = string.Join(", ", exercise.TargetedMuscles);

            BitmapImage exerciseImage = new BitmapImage(new Uri(exercise.Image, UriKind.RelativeOrAbsolute));
            exerciseImg.Source = exerciseImage;
            exerciseImagePopUp.Source = exercise.Image;
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
            if(contentArea != null)
            {
                contentArea.Content = new ExercisesControl();
            }
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            SystemSounds.Exclamation.Play();
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SqliteDataAccess.DeleteExercise(exercise.Id);
                MessageBox.Show("Exercise deleted successfully!");
                ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
                if (contentArea != null)
                {
                    contentArea.Content = new ExercisesControl();
                }
            }
        }

        private void addToWorkout_Click(object sender, RoutedEventArgs e)
        {
            ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
            if (contentArea != null)
            {
                contentArea.Content = new ChooseWorkoutControl(this, exercise);
            }
        }

        private void exerciseImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                exerciseImagePopUp.Show();
            }
        }

        private void _ExerciseInfoControl_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                backBtn_Click(sender, e);
            }
        }

        private void editExerciseBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
            if (contentArea != null)
            {
                contentArea.Content = new EditExerciseInfo(exercise);
            }
        }
    }
}
