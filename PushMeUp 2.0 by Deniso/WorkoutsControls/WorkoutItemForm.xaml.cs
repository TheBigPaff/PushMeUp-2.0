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
    /// Interaction logic for WorkoutItemForm.xaml
    /// </summary>
    public partial class WorkoutItemForm : UserControl
    {


        public Exercise Exercise
        {
            get { return (Exercise)GetValue(ExerciseProperty); }
            set { SetValue(ExerciseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Exercise.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExerciseProperty =
            DependencyProperty.Register("Exercise", typeof(Exercise), typeof(WorkoutItemForm), new PropertyMetadata(OnExercisePropertyChanged));
        private static void OnExercisePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((WorkoutItemForm)o).OnExercisePropertyChanged(e);
        }

        // set up the items on the combobox
        private void OnExercisePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            // IF THIS IS CALLED, THAT MEANS USER IS *NOT* EDITING

            equipmentSp.Visibility = Visibility.Visible;

            List<string> itemTypes = SqliteDataAccess.LoadWorkoutItemTypes();
            foreach (string itemType in itemTypes)
            {
                if (itemType.Equals("Break")) continue;
                itemTypesCb.Items.Add(itemType);
            }

            exerciseNameTb.Text = Exercise.Name;
        }


        public WorkoutItem WorkoutItem
        {
            get { return (WorkoutItem)GetValue(WorkoutItemProperty); }
            set { SetValue(WorkoutItemProperty, value); }
        }
        // Using a DependencyProperty as the backing store for WorkoutItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WorkoutItemProperty =
            DependencyProperty.Register("WorkoutItem", typeof(WorkoutItem), typeof(WorkoutItemForm), new PropertyMetadata(OnWorkoutPropertyChanged));
        private static void OnWorkoutPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((WorkoutItemForm)o).OnWorkoutPropertyChanged(e);
        }

        // set up the items on the combobox
        private void OnWorkoutPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            // IF THIS IS CALLED, THAT MEANS USER IS EDITING

            if (!String.IsNullOrWhiteSpace(WorkoutItem.EquipmentUsed))
            {
                equipmentTb.Text = WorkoutItem.EquipmentUsed;
            }

            if (WorkoutItem.ItemType == "Break")
            {
                exerciseNameTb.Text = "Break";
                equipmentSp.Visibility = Visibility.Collapsed;
            }
            else
            {
                exerciseNameTb.Text = WorkoutItem.Exercise.Name;

                equipmentSp.Visibility = Visibility.Visible;

                List<string> itemTypes = SqliteDataAccess.LoadWorkoutItemTypes();
                foreach (string itemType in itemTypes)
                {
                    if (itemType.Equals("Break")) continue;
                    itemTypesCb.Items.Add(itemType);
                }
            }

            switch (WorkoutItem.ItemType)
            {
                case "Break":
                    itemTypesCb.Items.Clear();
                    itemTypesCb.Items.Add("Break");
                    durationTb.Text = WorkoutItem.Duration.ToString();
                    break;

                case "Duration":
                    durationTb.Text = WorkoutItem.Duration.ToString();
                    break;

                case "Distance":
                    distanceTb.Text = WorkoutItem.Distance.ToString();
                    break;

                case "Reps":
                    repsTb.Text = WorkoutItem.Reps.ToString();
                    //setsTb.Text = WorkoutItem.Sets.ToString();
                    break;

                case "ToFailure":
                    break;
            }

            itemTypesCb.SelectedItem = WorkoutItem.ItemType;
        }

        public Workout Workout
        {
            get { return (Workout)GetValue(WorkoutProperty); }
            set { SetValue(WorkoutProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Workout.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WorkoutProperty =
            DependencyProperty.Register("Workout", typeof(Workout), typeof(WorkoutItemForm));


        public WorkoutItemForm()
        {
            InitializeComponent();
        }

        private void _WorkoutItemForm_Loaded(object sender, RoutedEventArgs e)
        {
            equipmentTipTb.Text = "In the 'Equipment Used' field you can insert everything about the equipment you use for this specific exercise.\n" +
                "For example, it can be '2lbs skipping rope', or 'barbell 50kgs each side'.\nAnything, without worrying about categories or units.";

            if(WorkoutItem == null)
            {
                distanceSp.Visibility = Visibility.Collapsed;
                durationSp.Visibility = Visibility.Collapsed;
                repsSp.Visibility = Visibility.Collapsed;
            }
        }

        private void itemTypesCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = itemTypesCb.SelectedItem.ToString();
            switch (selectedItem)
            {
                case "Break":
                case "Duration":
                    durationSp.Visibility = Visibility.Visible;

                    distanceSp.Visibility = Visibility.Collapsed;
                    repsSp.Visibility = Visibility.Collapsed;
                    break;

                case "Distance":
                    distanceSp.Visibility = Visibility.Visible;

                    durationSp.Visibility = Visibility.Collapsed;
                    repsSp.Visibility = Visibility.Collapsed;
                    break;

                case "Reps":
                    repsSp.Visibility = Visibility.Visible;

                    distanceSp.Visibility = Visibility.Collapsed;
                    durationSp.Visibility = Visibility.Collapsed;
                    break;

                case "ToFailure":
                    distanceSp.Visibility = Visibility.Collapsed;
                    durationSp.Visibility = Visibility.Collapsed;
                    repsSp.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private bool IsFormValid()
        {
            // any empty fields?
            if (itemTypesCb.SelectedIndex == -1 || (string.IsNullOrWhiteSpace(durationTb.Text) && durationSp.Visibility == Visibility.Visible)
                || (string.IsNullOrWhiteSpace(repsTb.Text) && repsSp.Visibility == Visibility.Visible)
                || (string.IsNullOrWhiteSpace(distanceTb.Text) && distanceSp.Visibility == Visibility.Visible))
            {
                MessageBox.Show("Some fields are empty.");
                return false;
            }
            else if ((!int.TryParse(durationTb.Text, out _) && durationSp.Visibility == Visibility.Visible)
                || (!int.TryParse(repsTb.Text, out _) && repsSp.Visibility == Visibility.Visible)
                || (!double.TryParse(distanceTb.Text, out _) && distanceSp.Visibility == Visibility.Visible))
            {
                MessageBox.Show("Only numbers inside!");
                return false;
            }
            //else if (((int.Parse(durationTb.Text) < 0) && durationSp.Visibility == Visibility.Visible)
            //    || ((int.Parse(repsTb.Text) < 0) && repsSetsSp.Visibility == Visibility.Visible) || ((int.Parse(setsTb.Text) < 0) && repsSetsSp.Visibility == Visibility.Visible)
            //    || ((int.Parse(distanceTb.Text) < 0) && distanceSp.Visibility == Visibility.Visible))
            //{
            //    MessageBox.Show("The values cannot be negative!");
            //    return false;
            //}
            else if (equipmentTb.Text.Length > 64)
            {
                MessageBox.Show("The 'Equipment used' field can't be longer than 64 characters.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if WorkoutItem was added to DB correctly. Otherwise false.
        /// </summary>
        internal bool AddWorkoutItemToDB()
        {
            if (!IsFormValid()) return false;

            // all good, add workout item
            WorkoutItem workoutItem;
            string selectedItem = itemTypesCb.SelectedItem.ToString();
            switch (selectedItem)
            {
                //case "Break":
                //    workoutItem = new WorkoutItem(selectedItem, int.Parse(durationTb.Text));
                //    break;
                case "Duration":
                    workoutItem = new WorkoutItem(selectedItem, int.Parse(durationTb.Text), Exercise);
                    break;

                case "Distance":
                    workoutItem = new WorkoutItem(selectedItem, double.Parse(distanceTb.Text), Exercise);
                    break;

                case "Reps":
                    workoutItem = new WorkoutItem(int.Parse(repsTb.Text), selectedItem, Exercise);
                    break;

                case "ToFailure":
                    workoutItem = new WorkoutItem(selectedItem, Exercise);
                    break;

                default:
                    workoutItem = new WorkoutItem();
                    break;
            }

            workoutItem.EquipmentUsed = equipmentTb.Text;
            SqliteDataAccess.SaveWorkoutItem(workoutItem, Workout);
            return true;
        }

        /// <summary>
        /// Returns true if WorkoutItem was added to DB correctly. Otherwise false.
        /// </summary>
        internal bool UpdateWorkoutItemToDB()
        {
            if (!IsFormValid()) return false;

            // all good, add workout item
            WorkoutItem workoutItem;
            string selectedItem = itemTypesCb.SelectedItem.ToString();
            switch (selectedItem)
            {
                case "Break":
                    workoutItem = new WorkoutItem(selectedItem, int.Parse(durationTb.Text));
                    break;
                case "Duration":
                    workoutItem = new WorkoutItem(selectedItem, int.Parse(durationTb.Text), Exercise);
                    break;

                case "Distance":
                    workoutItem = new WorkoutItem(selectedItem, double.Parse(distanceTb.Text), Exercise);
                    break;

                case "Reps":
                    workoutItem = new WorkoutItem(int.Parse(repsTb.Text), selectedItem, Exercise);
                    break;

                case "ToFailure":
                    workoutItem = new WorkoutItem(selectedItem, Exercise);
                    break;

                default:
                    workoutItem = new WorkoutItem();
                    break;
            }

            workoutItem.Id = WorkoutItem.Id;
            workoutItem.EquipmentUsed = equipmentTb.Text;
            SqliteDataAccess.UpdateWorkoutItem(workoutItem);
            return true;
        }

        private void equipmentUsedToolTipBtn_Click(object sender, RoutedEventArgs e)
        {
            if(equipmentTipGrid.Visibility == Visibility.Collapsed)
            {
                equipmentTipGrid.Visibility = Visibility.Visible;
            }
            else
            {
                equipmentTipGrid.Visibility = Visibility.Collapsed;
            }
        }
    }
}
