using CefSharp;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PushMeUp_2._0_by_Deniso
{
    /// <summary>
    /// Interaction logic for PlayingWorkoutControl.xaml
    /// </summary>
    public partial class PlayingWorkoutControl : UserControl
    {
        Workout workout;
        int currentExerciseIndex = 0;

        DateTime startingTime;
        DateTime endingTime;

        MediaPlayer beepPlayer;
        MediaPlayer finalBeepPlayer;
        MediaPlayer workoutComplete;

        MediaPlayer songMediaPlayer = new MediaPlayer();
        DispatcherTimer musicTimer;


        Timer countdownTimer;
        int countdownSeconds = 0;

        int selfRating = 1;
        
        //private string browserKeyword;
        //private bool firstTimeNavigating = true;

        //bool isBrowserInitialized = false;
        public PlayingWorkoutControl(Workout _workout)
        {
            InitializeComponent();

            workout = _workout;

            beepPlayer = new MediaPlayer();
            finalBeepPlayer = new MediaPlayer();
            workoutComplete = new MediaPlayer();


            /*
             * Before opening the mp3 files on the mediaplayers I set the volume at 0 because otherwise there's a weird bug that play the file for a millisecond when opening it 
             */
            beepPlayer.Volume = 0;
            finalBeepPlayer.Volume = 0;
            workoutComplete.Volume = 0;

            beepPlayer.Open(new Uri("./Assets/Sounds/beep.mp3", UriKind.Relative));
            finalBeepPlayer.Open(new Uri("./Assets/Sounds/final_beep.mp3", UriKind.Relative));
            workoutComplete.Open(new Uri("./Assets/Sounds/workoutComplete.mp3", UriKind.Relative));

            /*
             * Then I re-set it at default value (putting it at 0.8 coz it's too loud)
             */
            beepPlayer.Volume = 0.8;
            finalBeepPlayer.Volume = 0.8;
            workoutComplete.Volume = 0.8;

            musicTimer = new DispatcherTimer();
            musicTimer.Interval = TimeSpan.FromSeconds(1);
            musicTimer.Tick += DispatcherTimer_Tick;

            musicTimer.Start();

            countdownTimer = new Timer(1000);
            countdownTimer.Elapsed += CountdownTimer_Tick;

            volumeS.Value = 0.25; // default value

            
            //browser.IsBrowserInitializedChanged += Browser_IsBrowserInitializedChanged;
        }

        //private void Browser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    browser.LoadingStateChanged += Browser_LoadingStateChanged;
        //    NavigateToVideo();
        //}

        //public string Get(string uri)
        //{
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        //    request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        //    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //    using (Stream stream = response.GetResponseStream())
        //    using (StreamReader reader = new StreamReader(stream))
        //    {
        //        return reader.ReadToEnd();
        //    }
        //}

        //private void NavigateToVideo()
        //{
        //    string response = Get($"https://www.googleapis.com/youtube/v3/search?part=snippet&maxResults=20&q={browserKeyword}&type=video&key=AIzaSyDkAbQI4wquVqIc_ajpi4LEd5ZPYRLewfA");
        //    int startIndex = response.IndexOf("\"videoId\": \"") + "\"videoId\": \"".Length;
        //    int finalIndex = response.IndexOf("\"", startIndex);
        //    string videoId = response.Substring(startIndex, (finalIndex - startIndex));
        //    string url = "https://www.youtube.com/watch?v=" + videoId;

        //    //string url = $"https://www.youtube.com/results?search_query={browserKeyword}";
        //    browser.Load(url);
        //}

        //// loading spinning thing
        //private void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        //{
        //    if (!e.IsLoading)
        //    {
        //        this.Dispatcher.Invoke(() =>
        //        {
        //            loadingIcon.Visibility = Visibility.Collapsed;
        //        });
        //    }
        //    else
        //    {
        //        firstTimeNavigating = true;
        //    }
        //}


        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            countdownSeconds--;
            if(countdownSeconds > 0)
            {
                Dispatcher.BeginInvoke((Action)(() => {
                    beepPlayer.Play();
                    beepPlayer.Position = TimeSpan.Zero;

                    secondsCountdownTb.Text = countdownSeconds.ToString();
                }));
            }
            else if(countdownSeconds == 0)
            {
                Dispatcher.BeginInvoke((Action)(() => {
                    finalBeepPlayer.Play();

                    secondsCountdownTb.Text = countdownSeconds.ToString();
                }));
            }
            else
            {
                countdownTimer.Stop();

                currentExerciseIndex++;

                Dispatcher.BeginInvoke((Action)(() => {
                    ShowNewExercise();
                }));
            }
        }

        private void _PlayingWorkoutControl_Loaded(object sender, RoutedEventArgs e)
        {
            exerciseGrid.Visibility = Visibility.Collapsed;
            startWorkoutGrid.Visibility = Visibility.Visible;
        }
        private void startWorkoutBtn_Click(object sender, RoutedEventArgs e)
        {
            exerciseGrid.Visibility = Visibility.Visible;
            startWorkoutGrid.Visibility = Visibility.Collapsed;
            currentExerciseIndex = 0;

            startingTime = DateTime.Now;
            ShowNewExercise();
        }
        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            // if the user has finished his workout, just quit then
            if(startWorkoutGrid.Visibility == Visibility.Visible)
            {
                ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
                if (contentArea != null)
                {
                    countdownTimer.Stop();
                    songMediaPlayer.Stop();

                    contentArea.Content = new WorkoutInfoControl(workout);
                }
            }
            else
            {
                SystemSounds.Exclamation.Play();
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to quit the workout? Progress won't be saved.", "Quit Workout", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
                    if (contentArea != null)
                    {
                        countdownTimer.Stop();
                        songMediaPlayer.Stop();

                        contentArea.Content = new WorkoutInfoControl(workout);
                    }
                }
            }
        }

        private void timerControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            countdownTimer.Stop();
            SystemSounds.Exclamation.Play();
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to skip the exercise? Giving up is for losers!!!", "Skip exercise", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                currentExerciseIndex++;
                ShowNewExercise();
            }
            else
            {
                countdownTimer.Start();
            }
        }

        private void nextExerciseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (countdownTimer.Enabled)
            {
                countdownTimer.Stop();
                SystemSounds.Exclamation.Play();
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to skip the exercise? Giving up is for losers!!!", "Skip exercise", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    currentExerciseIndex++;
                    ShowNewExercise();
                }
                else
                {
                    countdownTimer.Start();
                }
            }
            else
            {
                currentExerciseIndex++;
                ShowNewExercise();
            }
        }

        private void previousExerciseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (countdownTimer.Enabled) countdownTimer.Stop();
            if (currentExerciseIndex > 0) currentExerciseIndex--;
            ShowNewExercise();
        }


        private void ShowNewExercise()
        {
            if (currentExerciseIndex < 0) return;
            // Workout finished
            if (workout.WorkoutItems.Count() <= currentExerciseIndex) 
            {
                WorkoutDone();
                return;
            }

            //if (!firstTimeNavigating)
            //{
            //    browser.Load("https://youtube.com");
            //}

            timerControl.Visibility = Visibility.Collapsed;

            WorkoutItem workoutItem = workout.WorkoutItems[currentExerciseIndex];

            if (workoutItem.ItemType.Equals("Break"))
            {
                exerciseImg.Source = new BitmapImage(new Uri(@"\Assets\timerImage.png", UriKind.RelativeOrAbsolute));

                //if (videoTab.IsSelected) imgTab.IsSelected = true;
                //videoTab.Visibility = Visibility.Collapsed;
            }
            else
            {
                exerciseImg.Source = new BitmapImage(new Uri(workoutItem.Exercise.Image, UriKind.RelativeOrAbsolute));

                //videoTab.Visibility = Visibility.Visible;

                //browserKeyword = workoutItem.Exercise.Name;
                //if (firstTimeNavigating)
                //{
                //    firstTimeNavigating = false;
                //}
                //else
                //{
                //    NavigateToVideo();
                //}
                //loadingIcon.Visibility = Visibility.Visible;
            }

            // SHOW TIMER
            if (workoutItem.ItemType.Equals("Duration") || workoutItem.ItemType.Equals("Break"))
            {
                secondsCountdownTb.Text = workoutItem.Duration.ToString();
                timerControl.Visibility = Visibility.Visible;

                countdownSeconds = workoutItem.Duration;
                countdownTimer.Start();
            }

            string instructions = "";
            switch (workoutItem.ItemType)
            {
                case "Break":
                    instructions = $"Rest for {workoutItem.Duration} seconds";
                    break;
                case "Distance":
                    instructions = $"{workoutItem.Exercise.Name} for {workoutItem.Distance} meters. Click the continue button when you're done!";
                    break;
                case "Reps":
                    instructions = $"{workoutItem.Exercise.Name} for {workoutItem.Reps} repetitions. Click the continue button when you're done!";
                    break;
                case "Duration":
                    instructions = $"{workoutItem.Exercise.Name} for {workoutItem.Duration} seconds";
                    break;
                case "ToFailure":
                    instructions = $"{workoutItem.Exercise.Name} until failure! Don't give up! Click the continue button when you're done!";
                    break;
            }
            instructionsTb.Text = instructions;
        }

        private string DurationToString(TimeSpan duration)
        {
            string durationString = "";

            durationString = workoutTime.Text = duration.Seconds + " seconds";
            if (duration.Minutes != 0)
            {
                durationString = duration.Minutes + " minutes, " + durationString;
                if (duration.Hours != 0)
                {
                    durationString = duration.Hours + " hours, " + durationString;
                }
            }

            return durationString;
        }

        private void WorkoutDone()
        {
            endingTime = DateTime.Now;
            //browser.WebBrowser.Delete();
            workoutComplete.Play();

            // check settings
            if (Session.Settings.IgnoreSelfRatingBool)
            {
                selfRatingBtn.IsChecked = false;
                selfRatingSp.Visibility = Visibility.Collapsed;
            }
            else
            {
                selfRatingBtn.IsChecked = true;
            }

            // set how long workout lasted
            string durationString = DurationToString(endingTime - startingTime);
            

            workoutTime.Text = durationString;
            endScreenGrid.Visibility = Visibility.Visible;
        }

        private void continueEndWorkout_Click(object sender, RoutedEventArgs e)
        {
            // get self rating
            if ((bool)selfRatingBtn.IsChecked)
            {
                selfRating = rating.Value;
            }
            else
            {
                selfRating = -1;
            }

            // get sendEmailReport
            if ((bool)emailReportBtn.IsChecked)
            {
                SendEmail();
            }

            // save workout for stats
            SqliteDataAccess.SaveCompletedWorkout(workout.Id, selfRating, startingTime, endingTime);

            ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
            if (contentArea != null)
            {
                songMediaPlayer.Stop();

                contentArea.Content = new WorkoutInfoControl(workout);
            }
        }

        private void SendEmail()
        {
            if (string.IsNullOrWhiteSpace(Session.LoggedUser.Email))
            {
                MessageBox.Show("You don't have an email. Set one first in the 'User Info' tab");
                return;
            }

            // make report
            string exercisesString = "";
            foreach (WorkoutItem item in workout.WorkoutItems)
            {
                string value = "";
                switch (item.ItemType)
                {
                    case "Break":
                    case "Duration":
                        value = ": " + item.Duration.ToString() + " seconds";
                        break;
                    case "Distance":
                        value = ": " + item.Distance.ToString() + " meters";
                        break;
                    case "Reps":
                        value = ": " + item.Reps.ToString() + " repetitions.";
                        break;
                }
                
                if(item.ItemType == "Break")
                {
                    exercisesString += $"{item.ItemType} {value}\n";
                }
                else
                {
                    exercisesString += $"{item.Exercise.Name} -> {item.ItemType} {value} | {item.EquipmentUsed}\n";
                }
            }

            string dateAndTimeString = endingTime.ToShortDateString() + " | " + endingTime.ToShortTimeString();
            string durationString = DurationToString(endingTime - startingTime);
            string title = $"Workout Report - PushMeUp 2.0";
            string body = "";
            if(selfRating != -1)
            {
                body = $"Hello {Session.LoggedUser.Username}!\nThis is the report you requested of the workout you did on {dateAndTimeString}\n\nExercises you did:\n{exercisesString}\nIt took you {durationString}!\nYou rated yourself {selfRating}*\n\nThanks for using PushMeUp 2.0!\n#DOTHETHING!";
            }
            else
            {
                body = $"Hello {Session.LoggedUser.Username}!\nThis is the report you requested of the workout you did on {dateAndTimeString}\n\nExercises you did:\n{exercisesString}\nAll of that took you {durationString}!\n\nThanks for using PushMeUp 2.0!\n#DOTHETHING!";
            }

            // send email report
            //sendingEmailIcon.Visibility = Visibility.Visible;

            // email user on a separate thread
            Task.Factory.StartNew(() => Helper.SendEmail(Session.LoggedUser.Email, title, body));
        }

        //private void DisableSendingEmailIcon(object sender, AsyncCompletedEventArgs e)
        //{
        //    sendingEmailIcon.Visibility = Visibility.Collapsed;
        //    MessageBox.Show("Email sent!");
        //}

        private void selfRatingBtn_CheckChanged(object sender, RoutedEventArgs e)
        {
            if((bool)selfRatingBtn.IsChecked)
            {
                selfRatingSp.Visibility = Visibility.Visible;
            }
            else
            {
                selfRatingSp.Visibility = Visibility.Collapsed;
            }
        }



        // MUSIC STUFF
        #region MUSIC
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (songMediaPlayer.Source != null)
            {
                if (songMediaPlayer.NaturalDuration.HasTimeSpan)
                {
                    timeTb.Text = songMediaPlayer.Position.ToString(@"mm\:ss") + "/" + songMediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
                }
            }
        }

        private void chooseMusic_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string pathMP3 = openFileDialog.FileName;
                songMediaPlayer.Open(new Uri(pathMP3));
            }
        }

        private void volumeS_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            songMediaPlayer.Volume = volumeS.Value;
        }

        private void playBtn_Click(object sender, RoutedEventArgs e)
        {
            songMediaPlayer.Play();
        }

        private void pauseBtn_Click(object sender, RoutedEventArgs e)
        {
            songMediaPlayer.Pause();
        }

        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {
            songMediaPlayer.Stop();
        }
        #endregion

    }
}
