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
    /// Interaction logic for EditWorkoutItem.xaml
    /// </summary>
    public partial class EditWorkoutItem : UserControl
    {
        WorkoutItem workoutItem;

        Workout workout;

        public EditWorkoutItem(int workoutItemId, Workout _workout)
        {
            InitializeComponent();

            workout = _workout;
            workoutItem = workout.WorkoutItems.Find(x => x.Id == workoutItemId);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            workoutItemForm.WorkoutItem = workoutItem;
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
            if (contentArea != null)
            {
                contentArea.Content = new WorkoutInfoControl(workout);
            }
        }

        private void editBtn_Click(object sender, RoutedEventArgs e)
        {
            if (workoutItemForm.UpdateWorkoutItemToDB())
            {
                MessageBox.Show($"Exercise updated succesfully!");
                backBtn_Click(sender, e);
            }
        }
    }
}
