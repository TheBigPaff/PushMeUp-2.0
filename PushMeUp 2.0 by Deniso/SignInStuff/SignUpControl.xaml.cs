using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
    /// Interaction logic for SignUpControl.xaml
    /// </summary>
    public partial class SignUpControl : UserControl
    {
        string proPicPath  = "";

        SignWindow parentWindow;
        public SignUpControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SignWindow signWindow = Helper.GetAncestorOfType<SignWindow>(this);
            if (signWindow != null)
            {
                parentWindow = signWindow;
            }


            proPicImg.Source = new BitmapImage(new Uri(@"/Assets/user_icon.png", UriKind.RelativeOrAbsolute));
            parentWindow.proPicImgPopUp.Source = "/Assets/user_icon.png";
        }


        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbUsername.Text) ||  string.IsNullOrWhiteSpace(pbPassword.Password) || string.IsNullOrWhiteSpace(dpBirthdate.Text) || string.IsNullOrWhiteSpace(tbHeight.Text) || string.IsNullOrWhiteSpace(tbWeight.Text))
            {
                MessageBox.Show("Some fields are empty");
                return;
            }
            else if (!double.TryParse(tbHeight.Text, out _) || !double.TryParse(tbWeight.Text, out _))
            {
                MessageBox.Show("Height and weight must contain numerical values");
                return;
            }
            else if (double.Parse(tbHeight.Text) < 0 || double.Parse(tbHeight.Text) > 500 || double.Parse(tbWeight.Text) < 0 || double.Parse(tbWeight.Text) > 500)
            {
                MessageBox.Show("Are you sure the height and weight values are correct?");
                return;
            }
            else if (tbUsername.Text.Length > 32 || pbPassword.Password.Length > 32)
            {
                MessageBox.Show("Usernames and password can't be longer than 32 characters");
                return;
            }
            else if (dpBirthdate.SelectedDate > DateTime.Today)
            {
                MessageBox.Show("You sure the birthdate is correct?");
                return;
            }
            else if (SqliteDataAccess.GetUserId(tbUsername.Text) != -1)
            {
                MessageBox.Show("A user with that name already exists.");
                return;
            }
            else if (string.IsNullOrWhiteSpace(tbEmail.Text))
            {
                SystemSounds.Exclamation.Play();
                MessageBoxResult _dialogResult = MessageBox.Show("If you don't insert an email then you won't be able to access your account in case you forget your password. Confirm empty email?", "No email", MessageBoxButton.YesNo);
                if (_dialogResult == MessageBoxResult.No)
                {
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(proPicPath)) proPicPath = "/Assets/user_icon.png";

            User user = new User()
            {
                Username = tbUsername.Text,
                Email = tbEmail.Text,
                Password = pbPassword.Password,
                ProPicPath = proPicPath,
                Birthdate = dpBirthdate.Text,
                Height = double.Parse(tbHeight.Text),
                Weight = double.Parse(tbWeight.Text)
            };

            Session.LoggedUser = SqliteDataAccess.RegisterUser(user);

            SqliteDataAccess.SaveBMIHistory(Session.LoggedUser.Id, double.Parse(tbHeight.Text), double.Parse(tbWeight.Text));

            // save default settings
            UserSettings settings = new UserSettings() { IgnoreSelfRating = 0, EmailReport = 0, StartupPage = "Workouts" };
            SqliteDataAccess.SaveSettings(settings);
            Session.Settings = settings;


            MessageBox.Show("User registered successfully!");

            MessageBoxResult dialogResult = MessageBox.Show("Do you want some default exercises to be added on your account already?", "Default exercises", MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                SaveDefaultExercises();
            }

            parentWindow.EnterApp(user.ProPicPath);
            
        }

        // Hardcoding stuff like this is VERY VERY bad. I should've done a DefaultExercisesTable on the Database (don't have time though).
        private void SaveDefaultExercises()
        {
            string pushUpDescription = @"1. Get down on all fours, placing your hands slightly wider than your shoulders. 
2. Straighten your arms and legs. 
3. Lower your body until your chest nearly touches the floor. 
4. Pause, then push yourself back up.";
            string runningDescription = @"Running is a type of gait characterized by an aerial phase in which all feet are above the ground (though there are exceptions). This is in contrast to walking, where one foot is always in contact with the ground, the legs are kept mostly straight and the center of gravity vaults over the stance leg or legs in an inverted pendulum fashion.";
            string benchPressdescription = @"1. Lie flat on your back on a bench.
2. Grip the bar with hands just wider than shoulder-width apart, so when you’re at the bottom of your move your hands are directly above your elbows. This allows for maximum force generation.
3. Bring the bar slowly down to your chest as you breathe in.
4. Push up as you breathe out, gripping the bar hard and watching a spot on the ceiling rather than the bar, so you can ensure it travels the same path every time.";
            string squatDescription = @"1. Stand with feet a little wider than hip width, toes facing front.

2. Drive your hips back—bending at the knees and ankles and pressing your knees slightly open—as you…

3. Sit into a squat position while still keeping your heels and toes on the ground, chest up and shoulders back.

4. Strive to eventually reach parallel, meaning knees are bent to a 90-degree angle.

5. Press into your heels and straighten legs to return to a standing upright position.";
            string jumpRopeDescription = @"";
            string shrugsDescription = @"Start with your feet flat on the floor, in a standing position. Your feet should be shoulder-width apart.
With your arms at your sides, turn your palms to face each other. If you’re doing the exercise with weights, bend down and grab them now.
Bend your knees slightly so that they line up with (not past) your toes. Keep your chin up, facing straight ahead, and your neck straight.
While you inhale, bring your shoulders as high up toward your ears as you can. Do the movement slowly so that you feel the resistance of your muscles.
Lower your shoulders back down and breathe out before repeating the movement.";

            SqliteDataAccess.SaveExercise(new Exercise("Push Up", pushUpDescription, "/ExerciseImages/pushup.jpg"), new List<string>{ "Chest", "Triceps", "Deltoids", "Abs"}, new List<string> { "Bodyweight" }); ;
            SqliteDataAccess.SaveExercise(new Exercise("Running", runningDescription, "/ExerciseImages/running.jpg"), new List<string> { "Quads", "Calves", "Glutes", "Harmstrings", "Obliques", "Abs" }, new List<string> { "Cardio", "Bodyweight" });
            SqliteDataAccess.SaveExercise(new Exercise("Bench Press", benchPressdescription, "/ExerciseImages/benchpress.jpg"), new List<string> { "Chest", "Triceps", "Abs" }, new List<string> { "Dumbbells", "Barbells", "Kettlebell" });
            SqliteDataAccess.SaveExercise(new Exercise("Squat", squatDescription, "/ExerciseImages/squat.jpg"), new List<string> { "Quads", "Calves", "Glutes", "Harmstrings" }, new List<string> { "Dumbbells", "Barbells", "Kettlebell", "Bodyweight" });
            SqliteDataAccess.SaveExercise(new Exercise("Jump Rope", jumpRopeDescription, "/ExerciseImages/jumprope.jpg"), new List<string> { "Forearms", "Quads", "Calves", "Glutes", "Hamstrings", "Traps", "Deltoids", "Abs" }, new List<string> { "Cardio" });
            SqliteDataAccess.SaveExercise(new Exercise("Shrugs", shrugsDescription, "/ExerciseImages/shrug.jpg"), new List<string> { "Neck", "Traps" }, new List<string> { "Dumbbells", "Barbells", "Kettlebell", "Bodyweight" });
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
                    proPicPath = openFileDialog.FileName;
                    proPicImg.Source = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.RelativeOrAbsolute));
                    parentWindow.proPicImgPopUp.Source = openFileDialog.FileName;
                }
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnSignUp_Click(sender, e);
            }
        }

        private void proPicImg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            parentWindow.proPicImgPopUp.Show();
        }
    }
}
