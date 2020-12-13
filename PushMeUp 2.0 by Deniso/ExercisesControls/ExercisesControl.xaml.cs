using PushMeUp_2._0_by_Deniso.CControls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PushMeUp_2._0_by_Deniso
{
    /// <summary>
    /// Interaction logic for ExercisesControl.xaml
    /// </summary>
    public partial class ExercisesControl : UserControl
    {
        List<Exercise> exercises;
        public ExercisesControl()
        {
            InitializeComponent();

            exercises = SqliteDataAccess.LoadExercises();
            LoadCardControls();
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

        private void LoadCardControls()
        {
            if(exercises.Count > 0)
            {
                noWorkoutsGrid.Visibility = Visibility.Collapsed;

                foreach (Exercise _exercise in exercises)
                {
                    cardsWp.Children.Add(new CardControl
                    {
                        ExerciseId = _exercise.Id,
                        ExerciseName = _exercise.Name,
                        ImageSource = new Uri(_exercise.Image, UriKind.RelativeOrAbsolute),
                    }); ;
                }
            }
            else
            {
                noWorkoutsGrid.Visibility = Visibility.Visible;
            }
        }

        public void ExerciseControlClicked(CardControl _exerciseControl)
        {
            Exercise exercise = exercises.Find(x => x.Id == _exerciseControl.ExerciseId);

            ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
            if (contentArea != null)
            {
                contentArea.Content = new ExerciseInfoControl(exercise);
            }
            
        }

        private void createNewBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
            if (contentArea != null)
            {
                contentArea.Content = new CreateNewExerciseControl()
                {
                    Width = contentArea.Width,
                    Height = contentArea.Height
                };
            }
        }

        private void SearchTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTb.Text)) LoadCardControls();
            if (exercises == null) return;

            cardsWp.Children.Clear();
            foreach (Exercise _exercise in exercises)
            {
                string searchText = SearchTb.Text.ToLower();

                // search algorithm
                if (_exercise.Name.ToLower().Contains(searchText) || _exercise.TargetedMuscles.Exists(x => x.ToLower().Contains(searchText)) || _exercise.ExerciseTypes.Exists(x => x.ToLower().Contains(searchText)))
                {
                    cardsWp.Children.Add(new CardControl
                    {
                        ExerciseId = _exercise.Id,
                        ExerciseName = _exercise.Name,
                        ImageSource = new Uri(_exercise.Image, UriKind.RelativeOrAbsolute),
                    }); ;
                }
            }
        }
    }
}
