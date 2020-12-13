using System;
using System.Collections.Generic;
using System.IO;
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
using CefSharp;
using CefSharp.Wpf;

namespace PushMeUp_2._0_by_Deniso
{
    /// <summary>
    /// Interaction logic for StatsControl.xaml
    /// </summary>
    public partial class StatsControl : UserControl
    {
        string calendarChartPath = "Charts/calendar.html";
        string columnsChartPath = "Charts/column_chart.html";
        string lineChartPath = "Charts/line_chart.html";
        string timeOfDayColumnsChartPath = "Charts/timeofday_column_chart.html"; // [{v: [8, 30], f: '8:30 AM'}, 2]

        bool useMinutes = true;

        public StatsControl()
        {
            InitializeComponent();

            browser.IsBrowserInitializedChanged += Browser_IsBrowserInitializedChanged;
            
            // loading user stats
            Session.LoggedUser.UserStats = SqliteDataAccess.LoadUserStats(Session.LoggedUser);
        }

        private void Browser_IsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            browser.LoadingStateChanged += Browser_LoadingStateChanged;
            DisplayChart();
        }

        // loading spinning thing
        private void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                this.Dispatcher.Invoke(() =>
                {
                    loadingIcon.Visibility = Visibility.Collapsed;
                });
            }
        }

        private void LoadHtml(string _html)
        {
            var base64EncodedHtml = Convert.ToBase64String(Encoding.UTF8.GetBytes(_html));
            browser.Load("data:text/html;base64," + base64EncodedHtml);
            loadingIcon.Visibility = Visibility.Visible;
        }

        private string GetCalendarChartHTML()
        {
            Dictionary<string, int> dateWorkoutCount = new Dictionary<string, int>(); // dictionary formed by each date of workout and number of workouts on that date
            

            string rows = "";
            // template : [ new Date(2012, 3, 13), 37032 ]
            foreach (UserStat userStat in Session.LoggedUser.UserStats)
            {
                if (!dateWorkoutCount.ContainsKey(userStat.Day))
                {
                    dateWorkoutCount.Add(userStat.Day, 1);
                }
                else
                {
                    dateWorkoutCount[userStat.Day]++;
                }
            }
            foreach (var item in dateWorkoutCount)
            {
                var splitDate = item.Key.Split('/');
                string day = splitDate[1];
                string month = (int.Parse(splitDate[0]) - 1).ToString(); // javascript month index begins at 0, so have to decrease month by 1
                string year = splitDate[2];

                rows += $"[ new Date({year}, {month}, {day}), {item.Value} ],";
            }

            if(rows.Length > 0)
            {
                rows = rows.Remove(rows.Length - 1); // remove last comma
            }

            string html = File.ReadAllText(calendarChartPath); // read "template" text

            // add actual data
            int startIndex = html.IndexOf("dataTable.addRows([") + "dataTable.addRows([".Length; 
            html = html.Insert(startIndex, rows);


            // return html with data
            return html;
        }


        private string GetMuscleColumnsChartHTML()
        {
            Dictionary<string, int> musclesHitCount = new Dictionary<string, int>(); // dictionary formed by muscle and number of times muscle was hit

            // set up dictionary
            List<string> muscleGroups = SqliteDataAccess.LoadMuscleGroups();
            foreach (string muscle in muscleGroups)
            {
                musclesHitCount.Add(muscle, 0);
            }

            string rows = "";
            // template for row: ['Triceps',  1],
            foreach (UserStat userStat in Session.LoggedUser.UserStats)
            {
                foreach (WorkoutItem workoutItem in userStat.completedWorkout.WorkoutItems)
                {
                    if(workoutItem.Exercise != null) // could be a "break"
                    {
                        foreach (string targetedMuscle in workoutItem.Exercise.TargetedMuscles)
                        {
                            musclesHitCount[targetedMuscle]++;
                        }
                    }
                }
            }

            foreach (var item in musclesHitCount)
            {
                rows += $"[ '{item.Key}', {item.Value}],";
            }

            if (rows.Length > 0)
            {
                rows = rows.Remove(rows.Length - 1); // remove last comma
            }

            string html = File.ReadAllText(columnsChartPath); // read "template" text

            // add actual data
            int startIndex = html.IndexOf("['Muscle', 'Times hit'],") + "['Muscle', 'Times hit'],".Length;
            html = html.Insert(startIndex, rows);


            // return html with data
            return html;
        }

        private string GetWeightLineChartHTML()
        {
            List<UserBMI> userBMIHistoryList = SqliteDataAccess.GetBMIHistoryOfUser(Session.LoggedUser.Id);

            // get the distinct value of weight
            userBMIHistoryList = userBMIHistoryList.GroupBy(x => x.Weight).Select(y => y.First()).ToList();

            string rows = "";
            //set up dictionary
            foreach (UserBMI record in userBMIHistoryList)
            {
                // get date in correct format
                var splitDate = record.Date.Split('/');
                string day = splitDate[1];
                string month = (int.Parse(splitDate[0]) - 1).ToString(); // javascript month index begins at 0, so have to decrease month by 1
                string year = splitDate[2];

                // get time in correct format
                string twentyfourBasedFormat = DateTime.Parse(record.Time).ToString("HH:mm");
                var splitTime = twentyfourBasedFormat.Split(':');
                string hours = splitTime[0];
                string minutes = splitTime[1];
                rows += $"[ new Date({year}, {month}, {day}, {hours}, {minutes}), {record.Weight} ],";
            }

            if (rows.Length > 0)
            {
                rows = rows.Remove(rows.Length - 1); // remove last comma
            }

            string html = File.ReadAllText(lineChartPath); // read "template" text
            html = html.Replace("BMI", "Weight");

            // add actual data
            int startIndex = html.IndexOf("data.addRows([") + "data.addRows([".Length;
            html = html.Insert(startIndex, rows);


            // return html with data
            return html;
        }

        private string GetBMILineChartHTML()
        {
            List<UserBMI> userBMIHistoryList = SqliteDataAccess.GetBMIHistoryOfUser(Session.LoggedUser.Id);

            string rows = "";
            //set up dictionary
            foreach (UserBMI record in userBMIHistoryList)
            {
                // get date in correct format
                var splitDate = record.Date.Split('/');
                string day = splitDate[1];
                string month = (int.Parse(splitDate[0]) - 1).ToString(); // javascript month index begins at 0, so have to decrease month by 1
                string year = splitDate[2];

                // get time in correct format
                string twentyfourBasedFormat = DateTime.Parse(record.Time).ToString("HH:mm");
                var splitTime = twentyfourBasedFormat.Split(':');
                string hours = splitTime[0];
                string minutes = splitTime[1];

                // calculate BMI
                double BMI = Math.Round((record.Weight / Math.Pow((record.Height / 100), 2)), 2);

                rows += $"[ new Date({year}, {month}, {day}, {hours}, {minutes}), {BMI} ],";
            }

            if (rows.Length > 0)
            {
                rows = rows.Remove(rows.Length - 1); // remove last comma
            }

            string html = File.ReadAllText(lineChartPath); // read "template" text

            // add actual data
            int startIndex = html.IndexOf("data.addRows([") + "data.addRows([".Length;
            html = html.Insert(startIndex, rows);


            // return html with data
            return html;
        }
        private string GetSelfRatingLineChartHTML()
        {
            List<UserStat> userStats = SqliteDataAccess.LoadUserStats(Session.LoggedUser);
            userStats.RemoveAll(x => x.SelfRating == -1);
            Dictionary<string, List<int>> dailySelfRating = new Dictionary<string, List<int>>(); // <date, List<rating>>

            string rows = "";
            //set up dictionary
            foreach (UserStat record in userStats)
            {

                if (!dailySelfRating.ContainsKey(record.Day))
                {
                    dailySelfRating.Add(record.Day, new List<int>());
                }
                dailySelfRating[record.Day].Add(record.SelfRating);
            }

            foreach (var item in dailySelfRating)
            {
                // get date in correct format
                var splitDate = item.Key.Split('/');
                string day = splitDate[1];
                string month = (int.Parse(splitDate[0]) - 1).ToString(); // javascript month index begins at 0, so have to decrease month by 1
                string year = splitDate[2];

                rows += $"[ new Date({year}, {month}, {day}), {(item.Value.Count > 0 ? item.Value.Average() : 0)} ],";
            }

            if (rows.Length > 0)
            {
                rows = rows.Remove(rows.Length - 1); // remove last comma
            }

            string html = File.ReadAllText(lineChartPath); // read "template" text
            string yAxisRange = @", minValue: 1.00, maxValue: 5.00";

            // add yAxisRange
            int startIndex = html.IndexOf("title: 'BMI'") + "title: 'BMI'".Length;
            html = html.Insert(startIndex, yAxisRange);

            // set the right value tag
            html = html.Replace("BMI", "Daily Average Self-Rating");

            // add rows
            startIndex = html.IndexOf("data.addRows([") + "data.addRows([".Length;
            html = html.Insert(startIndex, rows);


            // return html with data
            return html;
        }

        private string GetWorkoutsThroughoutDay()
        {
            Dictionary<DateTime, int> timesOfDay = new Dictionary<DateTime, int>(); // dictionary formed by <timeOfDay, numberOfWorkouts> 
            // each time of day has a 15 minutes difference


            // set up dictionary
            List<UserStat> userStats = SqliteDataAccess.LoadUserStats(Session.LoggedUser);
            foreach (UserStat userStat in userStats)
            {
                // get time rounded to nearer 15 minutes
                DateTime roundedTimeOfDay = Helper.RoundToNearest(DateTime.Parse(userStat.StartingTime), TimeSpan.FromMinutes(15));

                if (!timesOfDay.ContainsKey(roundedTimeOfDay))
                {
                    timesOfDay.Add(roundedTimeOfDay, 1);
                }
                else
                {
                    timesOfDay[roundedTimeOfDay]++;
                }
            }

            string rows = "";
            // template for row: [{v: [17, 30], f: '5:30 PM'}, 2],
            foreach (var item in timesOfDay)
            {
                rows += $"[{{v: [{item.Key.Hour}, {item.Key.Minute}], f: '{item.Key.ToShortTimeString()}'}}, {item.Value}],";
            }

            if (rows.Length > 0)
            {
                rows = rows.Remove(rows.Length - 1); // remove last comma
            }

            string html = File.ReadAllText(timeOfDayColumnsChartPath); // read "template" text

            // add actual data
            int startIndex = html.IndexOf("data.addRows([") + "data.addRows([".Length;
            html = html.Insert(startIndex, rows);


            // debugging
            File.WriteAllText(@"‪yes.html", html);

            // return html with data
            return html;
        }

        private string GetWorkoutLengthChartHTML()
        {
            List<UserStat> userStats = SqliteDataAccess.LoadUserStats(Session.LoggedUser);
            Dictionary<string, List<int>> dailyWorkoutLength = new Dictionary<string, List<int>>(); // <date, List<workoutDuration>>

            string rows = "";
            //set up dictionary
            foreach (UserStat record in userStats)
            {

                if (!dailyWorkoutLength.ContainsKey(record.Day))
                {
                    dailyWorkoutLength.Add(record.Day, new List<int>());
                }
                dailyWorkoutLength[record.Day].Add((int)((DateTime.Parse(record.FinishingTime) - DateTime.Parse(record.StartingTime)).TotalSeconds));
            }

            foreach (var item in dailyWorkoutLength)
            {
                // get date in correct format
                var splitDate = item.Key.Split('/');
                string day = splitDate[1];
                string month = (int.Parse(splitDate[0]) - 1).ToString(); // javascript month index begins at 0, so have to decrease month by 1
                string year = splitDate[2];

                if (useMinutes)
                {
                    rows += $"[ new Date({year}, {month}, {day}), {(item.Value.Select(x => x /= 60).Average())} ],";
                }
                else
                {
                    rows += $"[ new Date({year}, {month}, {day}), {(item.Value.Average())} ],";
                }
            }

            if (rows.Length > 0)
            {
                rows = rows.Remove(rows.Length - 1); // remove last comma
            }

            string html = File.ReadAllText(lineChartPath); // read "template" text

            // set the right value tag
            if (useMinutes)
            {
                html = html.Replace("BMI", "Daily Average Workouts Length (minutes)");
            }
            else
            {
                html = html.Replace("BMI", "Daily Average Workouts Length (seconds)");
            }

            // add rows
            int startIndex = html.IndexOf("data.addRows([") + "data.addRows([".Length;
            html = html.Insert(startIndex, rows);


            // return html with data
            return html;
        }

        private void DisplayChart()
        {
            if (browser.IsBrowserInitialized)
            {
                workoutDuration.Visibility = Visibility.Collapsed;

                switch (tbc.SelectedIndex)
                {
                    case 0:
                        LoadHtml(GetCalendarChartHTML());
                        break;
                    case 1:
                        LoadHtml(GetMuscleColumnsChartHTML());
                        break;
                    case 2:
                        LoadHtml(GetWeightLineChartHTML());
                        break;
                    case 3:
                        LoadHtml(GetBMILineChartHTML());
                        break;
                    case 4:
                        LoadHtml(GetSelfRatingLineChartHTML());
                        break;
                    case 5:
                        LoadHtml(GetWorkoutsThroughoutDay());
                        break;
                    case 6:
                        LoadHtml(GetWorkoutLengthChartHTML());
                        workoutDuration.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void tbc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayChart();
        }

        private void changeDurationUnit_Click(object sender, RoutedEventArgs e)
        {
            if (useMinutes)
            {
                useMinutes = false;
                changeDurationUnitBtn.Content = "Seconds";
                DisplayChart();
            }
            else
            {
                useMinutes = true;
                changeDurationUnitBtn.Content = "Minutes";
                DisplayChart();
            }
        }



        //private void okBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    toolTipBox.Visibility = Visibility.Collapsed;
        //}

        //private void backBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    ContentControl contentArea = Helper.GetAncestorOfType<ContentControl>(this);
        //    if (contentArea != null)
        //    {
        //        contentArea.Content = new WorkoutsControl();
        //    }
        //}

        //private void helpBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    toolTipTb.Text = "These are the charts with stats based on your completed workouts. You can change chart clicking a different tab on top.";
        //    toolTipBox.Visibility = Visibility.Visible;
        //}
    }
}
