using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace PushMeUp_2._0_by_Deniso
{
    /// <summary>
    /// Interaction logic for CreateNewExerciseControl.xaml
    /// </summary>
    public partial class CreateNewExerciseControl : UserControl
    {
        public CreateNewExerciseControl()
        {
            InitializeComponent();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
            if (contentArea != null)
            {
                contentArea.Content = new ExercisesControl();
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            exerciseForm.AddExerciseToDB();
        }
    }
}
