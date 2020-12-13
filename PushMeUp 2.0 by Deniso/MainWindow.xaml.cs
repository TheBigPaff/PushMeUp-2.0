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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isWindowMaximized = false;

        public MainWindow(double _leftPosition, double _topPosition)
        {
            InitializeComponent();

            this.Left = _leftPosition;
            this.Top = _topPosition;

            Session.Settings = SqliteDataAccess.LoadSettings();

            SetStartupPage();

            usernameBtn.Content = Session.LoggedUser.Username;
        }

        private void SetStartupPage()
        {
            UserControl startupPage;
            switch (Session.Settings.StartupPage)
            {
                case "User Info":
                    startupPage = new UserInfoControl();
                    usernameBtn.IsChecked = true;
                    break;
                case "Workouts":
                    startupPage = new WorkoutsControl();
                    MyWorkoutBtn.IsChecked = true;
                    break;
                case "Exercises":
                    startupPage = new ExercisesControl();
                    ExercisesBtn.IsChecked = true;
                    break;
                case "Statistics":
                    startupPage = new StatsControl();
                    StatsBtn.IsChecked = true;
                    break;
                case "Settings":
                    startupPage = new SettingsControl();
                    SettingsBtn.IsChecked = true;
                    break;
                default:
                    startupPage = new WorkoutsControl();
                    break;
            }

            ContentArea.Content = startupPage;
        }

        public Uri ImageSource
        {
            get { return (Uri)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
        // Using a DependencyProperty as the backing store for ImageSource. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(Uri), typeof(MainWindow));

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void MyWorkoutBtn_Click(object sender, RoutedEventArgs e)
        {
            UserControl myWorkouts = new WorkoutsControl();
            ContentArea.Content = myWorkouts;
        }

        private void ExercisesBtn_Click(object sender, RoutedEventArgs e)
        {
            UserControl exercisesControl = new ExercisesControl();
            ContentArea.Content = exercisesControl;
        }

        private void logoutBtn_Click(object sender, RoutedEventArgs e)
        {
            new SignWindow(this.Left, this.Top).Show();
            this.Close();
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void minimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void maximizeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                isWindowMaximized = true;
            }
            
            if (isWindowMaximized)
            {
                this.Height = 745;
                this.Width = 1602;

                isWindowMaximized = false;
            }
            else
            {
                this.Left = SystemParameters.WorkArea.Left;
                this.Top = SystemParameters.WorkArea.Top;
                this.Height = SystemParameters.WorkArea.Height;
                this.Width = SystemParameters.WorkArea.Width;

                isWindowMaximized = true;
            }

        }

        private void StatsBtn_Checked(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new StatsControl();
        }

        private void usernameBtn_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new UserInfoControl();
        }

        private void SupportBtn_Checked(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new SupportControl();
        }

        private void SettingsBtn_Checked(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new SettingsControl();
        }

        private void BackupBtn_Checked(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new BackupControl();
        }
    }
}
