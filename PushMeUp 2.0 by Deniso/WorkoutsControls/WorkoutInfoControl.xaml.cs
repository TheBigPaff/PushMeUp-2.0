using PushMeUp_2._0_by_Deniso.CControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for WorkoutInfoControl.xaml
    /// </summary>
    public partial class WorkoutInfoControl : UserControl
    {
        Workout workout;

        public WorkoutInfoControl(Workout _workout)
        {
            InitializeComponent();

            workout = _workout;
        }

        private void _WorkoutInfoControl_Loaded(object sender, RoutedEventArgs e)
        {
            //addExerciseExplanationTb.Text = "Add an exercise -> go to the exercises tab, choose the exercise you want to add, then select \'Add to a workout\' on the top right, and then choose the workout to add the exercise to." +
            //    "\n\nAdd break -> click blue button 'Add Break' on the right.\n\nRemove an exercise -> Right click on it and then click on 'Delete'.\n\nEdit an exercise -> Right click on it and then click on 'Edit'." +
            //    "\n\nRe-arrange items by dragging and dropping them where you want them.";

            explanationTb.Inlines.Clear();
            explanationTb.Inlines.Add(new Run("Add an exercise") { Foreground = Brushes.Teal });
            explanationTb.Inlines.Add(new Run(" -> Go to the 'Exercise' tab, choose the exercise you want to add, then select \'Add to a workout\' on the top right, and then choose the workout to add the exercise to."));

            explanationTb.Inlines.Add(new Run("\n\nAdd break") { Foreground = Brushes.Teal });
            explanationTb.Inlines.Add(new Run(" -> Click blue button 'Add Break' on the right."));

            explanationTb.Inlines.Add(new Run("\n\nRemove an exercise") { Foreground = Brushes.Teal });
            explanationTb.Inlines.Add(new Run(" -> Right click on exercise and then click on 'Remove'."));

            explanationTb.Inlines.Add(new Run("\n\nEdit an exercise") { Foreground = Brushes.Teal });
            explanationTb.Inlines.Add(new Run(" -> Right click on exercise and then click on 'Edit'."));

            explanationTb.Inlines.Add(new Run("\n\nRe-arrange items") { Foreground = Brushes.Teal });
            explanationTb.Inlines.Add(new Run(" -> Drag and drop them where you want them."));

            workoutNameTb.Text = workout.Name;

            UpdateStackPanel();
        }

        private void editWorkoutNameBtn_Click(object sender, RoutedEventArgs e)
        {
            if (workoutNameTb.IsReadOnly)
            {
                // user is not editing, wants to edit
                workoutNameTb.IsReadOnly = false;
                workoutNameTb.BorderThickness = new Thickness(0, 0, 0, 1);
                Keyboard.Focus(workoutNameTb);
            }
            else
            {
                // user is already editing, wants to save
                if(workoutNameTb.Text != workout.Name)
                {
                    // if he actually changed the name

                    // validate input
                    if (workoutNameTb.Text.Length > 20)
                    {
                        MessageBox.Show("Workout name can't be longer than 20 characters");
                        return;
                    }

                    //update it
                    SqliteDataAccess.UpdateWorkoutName(workout.Id, workoutNameTb.Text);
                    workout.Name = workoutNameTb.Text;
                }
                workoutNameTb.IsReadOnly = true;
                workoutNameTb.BorderThickness = new Thickness(0, 0, 0, 0);
            }

        }
        private void workoutNameTb_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                editWorkoutNameBtn_Click(sender, e);
                Keyboard.ClearFocus();
            }
        }

        private void UpdateStackPanel()
        {
            // clear old ones
            itemsSp.Children.Clear();

            workout = SqliteDataAccess.LoadWorkout(workout.Id);
            // add new ones
            foreach (WorkoutItem workoutItem in workout.WorkoutItems)
            {
                if (workoutItem.ItemType.Equals("Break"))
                {
                    itemsSp.Children.Add(new WorkoutItemControl(workoutItem.Id)
                    {
                        ExerciseName = "Break",
                        ImageSource = new Uri(@".\Assets\timerImage.png", UriKind.RelativeOrAbsolute),
                        ItemType = workoutItem.ItemType,
                        ItemTypeValue = workoutItem.Duration.ToString() + " seconds"
                    });
                }
                else
                {
                    string value;
                    if (workoutItem.ItemType.Equals("Distance"))
                    {
                        value = workoutItem.Distance.ToString() + " meters";
                    }
                    else if (workoutItem.ItemType.Equals("Reps")){
                        value = workoutItem.Reps.ToString() + " repetitions";
                    }
                    else if (workoutItem.ItemType.Equals("Duration"))
                    {
                        value = workoutItem.Duration.ToString() + " seconds";
                    }
                    else
                    {
                        value = "";
                    }
                    itemsSp.Children.Add(new WorkoutItemControl(workoutItem.Id)
                    {
                        ExerciseName = workoutItem.Exercise.Name,
                        ImageSource = new Uri(workoutItem.Exercise.Image, UriKind.RelativeOrAbsolute),
                        ItemType = workoutItem.ItemType,
                        ItemTypeValue = value,
                        EquipmentUsed = workoutItem.EquipmentUsed
                    });
                }
            }
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            SystemSounds.Exclamation.Play();
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SqliteDataAccess.DeleteWorkout(workout.Id);
                MessageBox.Show("Exercise deleted successfully!");
                ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
                if (contentArea != null)
                {
                    contentArea.Content = new WorkoutsControl();
                }
            }
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
            if (contentArea != null)
            {
                contentArea.Content = new WorkoutsControl();
            }
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Start workout?", "Workout Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                // TODO: Start Workout!!! :D
                ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
                if (contentArea != null)
                {
                    contentArea.Content = new PlayingWorkoutControl(workout);
                }
            }
        }

        private void addBreakBtn_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Visibility = Visibility.Visible;
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            // YesButton Clicked! Let's hide our InputBox and handle the input text.
            InputBox.Visibility = Visibility.Collapsed;

            // Do something with the Input
            int input;
            if (string.IsNullOrWhiteSpace(InputTextBox.Text) || !int.TryParse(InputTextBox.Text, out input))
            {
                MessageBox.Show("Empty field or not an integer.");
            }
            else if (int.Parse(InputTextBox.Text) < 0)
            {
                MessageBox.Show("The duration of the break cannot be negative");
            }
            else
            {
                workout.WorkoutItems.Add(new WorkoutItem("Break", input));
                SqliteDataAccess.UpdateWorkout(workout);
                UpdateStackPanel();
            }

            // Clear InputBox.
            InputTextBox.Text = string.Empty;
        }
        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            // NoButton Clicked! Let's hide our InputBox.
            InputBox.Visibility = Visibility.Collapsed;

            // Clear InputBox.
            InputTextBox.Text = string.Empty;
        }

        private void helpBtn_Click(object sender, RoutedEventArgs e)
        {
            toolTipTb.Text = "Don't know what to do? Need further help? Contact the developer by going to the 'Support' tab.";
            toolTipBox.Visibility = Visibility.Visible;
        }
        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            toolTipBox.Visibility = Visibility.Collapsed;
        }

        internal void DeleteItem(int id)
        {
            workout.WorkoutItems.Remove(workout.WorkoutItems.Find(x => x.Id == id));
            SqliteDataAccess.UpdateWorkout(workout);
            UpdateStackPanel();
        }

        internal void EditItem(int workoutItemId)
        {
            ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
            if (contentArea != null)
            {
                contentArea.Content = new EditWorkoutItem(workoutItemId, workout);
            }
        }

        #region DRAG AND DROP STUFF

        private bool _isDown;
        private bool _isDragging;
        private Point _startPoint;
        private UIElement _realDragSource;
        private UIElement _dummyDragSource = new UIElement();

        private void sp_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == this.itemsSp)
            {
            }
            else
            {
                _isDown = true;
                _startPoint = e.GetPosition(this.itemsSp);
            }
        }

        private void sp_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDown = false;
            _isDragging = false;
            if(_realDragSource != null)
                _realDragSource.ReleaseMouseCapture();
        }

        private void sp_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDown)
            {
                if ((_isDragging == false) && ((Math.Abs(e.GetPosition(this.itemsSp).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(e.GetPosition(this.itemsSp).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                {
                    _isDragging = true;
                    _realDragSource = e.Source as UIElement;
                    _realDragSource.CaptureMouse();
                    DragDrop.DoDragDrop(_dummyDragSource, new DataObject("UIElement", e.Source, true), DragDropEffects.Move);
                }
            }
        }

        private void sp_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("UIElement"))
            {
                e.Effects = DragDropEffects.Move;
            }
        }

        private void sp_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("UIElement"))
            {
                UIElement droptarget = e.Source as UIElement;
                int droptargetIndex = -1, i = 0;
                foreach (UIElement element in this.itemsSp.Children)
                {
                    if (element.Equals(droptarget))
                    {
                        droptargetIndex = i;
                        break;
                    }
                    i++;
                }
                if (droptargetIndex != -1)
                {
                    this.itemsSp.Children.Remove(_realDragSource);
                    this.itemsSp.Children.Insert(droptargetIndex, _realDragSource);


                    // UPDATE ARRANGEMENT ON workout LIST
                    workout.WorkoutItems.Move<WorkoutItem>(workout.WorkoutItems.FindIndex(x => x.Id == (_realDragSource as WorkoutItemControl).workoutItemId), workout.WorkoutItems.FindIndex(x => x.Id == (droptarget as WorkoutItemControl).workoutItemId));
                }

                _isDown = false;
                _isDragging = false;
                _realDragSource.ReleaseMouseCapture();

                // UPDATE NEW RE-ARRANGEMENT LIST
                
                SqliteDataAccess.UpdateWorkout(workout);
            }
        }

        #endregion
    }
}
