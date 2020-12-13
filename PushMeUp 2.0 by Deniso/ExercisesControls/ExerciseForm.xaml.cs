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
using _WinForms = System.Windows.Forms;

namespace PushMeUp_2._0_by_Deniso
{
    /// <summary>
    /// Interaction logic for ExerciseForm.xaml
    /// </summary>
    public partial class ExerciseForm : UserControl
    {
        #region DependencyProperties
        public int ExerciseId
        {
            get { return (int)GetValue(ExerciseIdProperty); }
            set { SetValue(ExerciseIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExerciseId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExerciseIdProperty =
            DependencyProperty.Register("ExerciseId", typeof(int), typeof(ExerciseForm));

        public string ExerciseName
        {
            get { return (string)GetValue(ExerciseNameProperty); }
            set { SetValue(ExerciseNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExerciseNameProperty =
            DependencyProperty.Register("ExerciseName", typeof(string), typeof(ExerciseForm), new PropertyMetadata("", OnExerciseNamePropertyChanged));

        private static void OnExerciseNamePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ExerciseForm)o).OnExerciseNamePropertyChanged(e);
        }
        // set up the items on the combobox
        private void OnExerciseNamePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            tbName.Text = ExerciseName;
        }

        public string ExerciseDescription
        {
            get { return (string)GetValue(ExerciseDescriptionProperty); }
            set { SetValue(ExerciseDescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExerciseDescription.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExerciseDescriptionProperty =
            DependencyProperty.Register("ExerciseDescription", typeof(string), typeof(ExerciseForm), new PropertyMetadata("", OnExerciseDescriptionPropertyChanged));
        private static void OnExerciseDescriptionPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ExerciseForm)o).OnExerciseDescriptionPropertyChanged(e);
        }
        // set up the items on the combobox
        private void OnExerciseDescriptionPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            tbDescription.Text = ExerciseDescription;
        }
        public string ExerciseImagePath
        {
            get { return (string)GetValue(ExerciseImagePathProperty); }
            set { SetValue(ExerciseImagePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExerciseImagePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExerciseImagePathProperty =
            DependencyProperty.Register("ExerciseImagePath", typeof(string), typeof(ExerciseForm), new PropertyMetadata(""));

        public List<string> MuscleGroups
        {
            get { return (List<string>)GetValue(MuscleGroupsProperty); }
            set { SetValue(MuscleGroupsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MuscleGroups.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MuscleGroupsProperty =
            DependencyProperty.Register("MuscleGroups", typeof(List<string>), typeof(ExerciseForm), new PropertyMetadata(new List<string>(), OnMuscleGroupsPropertyChanged));

        private static void OnMuscleGroupsPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ExerciseForm)o).OnMuscleGroupsPropertyChanged(e);
        }

        // set up the items on the combobox
        private void OnMuscleGroupsPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            muscleGroupsCb.SetCheckedItems(MuscleGroups);
        }


        public List<string> ExerciseTypes
        {
            get { return (List<string>)GetValue(ExerciseTypesProperty); }
            set { SetValue(ExerciseTypesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExerciseTypes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExerciseTypesProperty =
            DependencyProperty.Register("ExerciseTypes", typeof(List<string>), typeof(ExerciseForm), new PropertyMetadata(new List<string>(), OnExerciseTypesPropertyChanged));
        private static void OnExerciseTypesPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((ExerciseForm)o).OnExerciseTypesPropertyChanged(e);
        }

        // set up the items on the combobox
        private void OnExerciseTypesPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            exerciseTypeCb.SetCheckedItems(ExerciseTypes);
        }
        #endregion

        public ExerciseForm()
        {
            InitializeComponent();
        }

        private void _ExerciseForm_Loaded(object sender, RoutedEventArgs e)
        {
            muscleGroupsCb.comboBoxItems = SqliteDataAccess.LoadMuscleGroups();
            exerciseTypeCb.comboBoxItems = SqliteDataAccess.LoadExerciseTypes();
        }

        private void btnOpenImageDialog_Click(object sender, RoutedEventArgs e)
        {
            using (_WinForms.OpenFileDialog openFileDialog = new _WinForms.OpenFileDialog())
            {
                openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == _WinForms.DialogResult.OK)
                {
                    //Get the path of specified file
                    ExerciseImagePath = openFileDialog.FileName;
                }
            }
        }

        private void GoToExerciseInfo(Exercise exercise)
        {
            ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
            if (contentArea != null)
            {
                contentArea.Content = new ExerciseInfoControl(exercise);
            }
        }

        private bool IsFormValid()
        {
            if (string.IsNullOrWhiteSpace(ExerciseImagePath))
            {
                ExerciseImagePath = "/Assets/default_exercise_icon.png";
            }

            // any empty fields?
            if (string.IsNullOrWhiteSpace(tbName.Text))
            {
                MessageBox.Show("The name field cannot be empty.");
                return false;
            }
            else if (tbName.Text.Length > 16)
            {
                MessageBox.Show("Name can't be longer than 16 characters.");
                return false;
            }
            else if (tbDescription.Text.Length > 1024)
            {
                MessageBox.Show("Description can't be longer than 1024 characters.");
                return false;
            }
            else
            {
                return true;
            }
        }

        internal void AddExerciseToDB()
        {
            if (!IsFormValid()) return;

            // check if exercise with that name already exists
            if (SqliteDataAccess.DoesExerciseExist(tbName.Text))
            {
                MessageBox.Show("Exercise with that name already exists.");
                return;
            }

            List<string> muscleGroups = muscleGroupsCb.GetCheckedItems();
            List<string> exerciseTypes = exerciseTypeCb.GetCheckedItems();


            Exercise exerciseToAdd = new Exercise(ExerciseId, tbName.Text, tbDescription.Text, ExerciseImagePath);
            exerciseToAdd = SqliteDataAccess.SaveExercise(exerciseToAdd, muscleGroups, exerciseTypes);
            
            MessageBox.Show("Exercise added successfully!");
            GoToExerciseInfo(exerciseToAdd);
        }

        internal void EditExercise()
        {
            if (!IsFormValid()) return;

            // if user has changed the exercise's name, check if it already exists
            if (ExerciseName != tbName.Text && SqliteDataAccess.DoesExerciseExist(tbName.Text))
            {
                MessageBox.Show("Exercise with that name already exists.");
                return;
            }

            List<string> muscleGroups = muscleGroupsCb.GetCheckedItems();
            List<string> exerciseTypes = exerciseTypeCb.GetCheckedItems();

            Exercise exerciseToUpdate = new Exercise(ExerciseId, tbName.Text, tbDescription.Text, ExerciseImagePath);
            SqliteDataAccess.UpdateExercise(ExerciseId, exerciseToUpdate, muscleGroups, exerciseTypes);
            exerciseToUpdate = SqliteDataAccess.LoadExercise(exerciseToUpdate.Id);

            MessageBox.Show("Exercise updated successfully!");
            GoToExerciseInfo(exerciseToUpdate);
        }
    }
}
