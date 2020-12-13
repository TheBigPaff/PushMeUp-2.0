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
    /// Interaction logic for CardControl.xaml
    /// </summary>
    public partial class CardControl : UserControl
    {
        public int ExerciseId;
        public CardControl()
        {
            InitializeComponent();
        }

        public Uri ImageSource
        {
            get { return (Uri)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ImageSource. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(Uri), typeof(CardControl));

        public string ExerciseName
        {
            get { return (string)GetValue(ExerciseNameProperty); }
            set { SetValue(ExerciseNameProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ExerciseName. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExerciseNameProperty = DependencyProperty.Register("ExerciseName", typeof(string), typeof(CardControl));



        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(CardControl));

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            ExercisesControl parent = Helper.GetAncestorOfType<ExercisesControl>(this);
            if(parent != null)
            {
                parent.ExerciseControlClicked(this);
            }
        }
    }
}
