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
    /// Interaction logic for ChooseWorkoutControl.xaml
    /// </summary>
    public partial class ChooseWorkoutControl : UserControl
    {
        ExerciseInfoControl parent;
        Exercise exercise;
        public ChooseWorkoutControl(ExerciseInfoControl _parent, Exercise _exercise)
        {
            InitializeComponent();

            parent = _parent;
            exercise = _exercise;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new WorkoutsControl();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
            if (contentArea != null)
            {
                contentArea.Content = parent;
            }
        }

        public void AddToWorkout(Workout _workout)
        {
            ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
            if (contentArea != null)
            {
                contentArea.Content = new CreateWorkoutItemControl(_workout, exercise, parent);
            }
        }
    }
}
