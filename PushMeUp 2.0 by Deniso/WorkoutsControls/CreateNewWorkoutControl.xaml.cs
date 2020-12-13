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
    /// Interaction logic for CreateNewWorkoutControl.xaml
    /// </summary>
    public partial class CreateNewWorkoutControl : UserControl
    {
        public CreateNewWorkoutControl()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // any empty fields?
            if (string.IsNullOrWhiteSpace(tbName.Text))
            {
                MessageBox.Show("Name can't be empty.");
                return;
            }
            else if(tbName.Text.Length > 20)
            {
                MessageBox.Show("Name can't be longer than 20 characters.");
                return;
            }

            // all good, add workout

            SqliteDataAccess.SaveWorkout(tbName.Text);


            MessageBox.Show("Workout added successfully!");
            backBtn_Click(sender, e);
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
            if (contentArea != null)
            {
                contentArea.Content = new WorkoutsControl();
            }
        }
    }
}
