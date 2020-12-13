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
    /// Interaction logic for EditExerciseInfo.xaml
    /// </summary>
    public partial class EditExerciseInfo : UserControl
    {
        Exercise exercise;
        public EditExerciseInfo(Exercise _exercise)
        {
            InitializeComponent();

            exercise = _exercise;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            exerciseForm.ExerciseId = exercise.Id;
            exerciseForm.ExerciseName = exercise.Name;
            exerciseForm.ExerciseDescription = exercise.Description;
            exerciseForm.ExerciseImagePath = exercise.Image;
            exerciseForm.Loaded += SetCheckedItems;
        }

        private void SetCheckedItems(object sender, RoutedEventArgs e)
        {
            exerciseForm.MuscleGroups = exercise.TargetedMuscles;
            exerciseForm.ExerciseTypes = exercise.ExerciseTypes;
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            exerciseForm.EditExercise();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
            if (contentArea != null)
            {
                contentArea.Content = new ExerciseInfoControl(exercise);
            }
        }
    }
}
