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

namespace PushMeUp_2._0_by_Deniso.CControls
{
    /// <summary>
    /// Interaction logic for WorkoutControl.xaml
    /// </summary>
    public partial class WorkoutControl : UserControl
    {
        public int WorkoutId;
        public WorkoutControl()
        {
            InitializeComponent();
        }

        public Uri ImageSource
        {
            get { return (Uri)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ImageSource. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(Uri), typeof(WorkoutControl));

        public string WorkoutName
        {
            get { return (string)GetValue(WorkoutNameProperty); }
            set { SetValue(WorkoutNameProperty, value); }
        }
        // Using a DependencyProperty as the backing store for WorkoutName. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WorkoutNameProperty = DependencyProperty.Register("WorkoutName", typeof(string), typeof(WorkoutControl));



        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(WorkoutControl));



        public string ExercisesCount
        {
            get { return (string)GetValue(ExercisesCountProperty); }
            set { SetValue(ExercisesCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExercisesCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExercisesCountProperty = DependencyProperty.Register("ExercisesCount", typeof(string), typeof(WorkoutControl));

        public string BreaksCount
        {
            get { return (string)GetValue(BreaksCountProperty); }
            set { SetValue(BreaksCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BreaksCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BreaksCountProperty = DependencyProperty.Register("BreaksCount", typeof(string), typeof(WorkoutControl));

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            WorkoutsControl parent = Helper.GetAncestorOfType<WorkoutsControl>(this);
            if (parent != null)
            {
                parent.WorkoutControlClicked(this);
            }
        }
    }
}
