using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PushMeUp_2._0_by_Deniso
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        

        Timer settingSavedCheckIconTimer;

        public SettingsControl()
        {
            InitializeComponent();

            settingSavedCheckIconTimer = new Timer();
            settingSavedCheckIconTimer.Interval = 2000;
            settingSavedCheckIconTimer.Elapsed += SettingSavedCheckIconTimer_Elapsed;

            SetSettings();
        }

        private void SetSettings()
        {
            startupPageCb.SelectedItem = startupPageCb.FindName(Session.Settings.StartupPage.Replace(" ", ""));
            selfRatingBtn.IsChecked = Session.Settings.IgnoreSelfRating == 1;
            emailReportBtn.IsChecked = Session.Settings.EmailReport == 1;
        }

        private void SettingSavedCheckIconTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => {
                settingSavedCheckSp.Visibility = Visibility.Collapsed;
            }));

            settingSavedCheckIconTimer.Stop();
        }

        private void SaveSetting()
        {
            SqliteDataAccess.UpdateSettings(Session.Settings);

            if(settingSavedCheckSp != null)
            {
                settingSavedCheckSp.Visibility = Visibility.Visible;
                settingSavedCheckIconTimer.Start();
            }
        }

        private void startupPageCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // if user changed settings
            if(Session.Settings.StartupPage != (startupPageCb.SelectedItem as ComboBoxItem).Content.ToString())
            {
                Session.Settings.StartupPage = (startupPageCb.SelectedItem as ComboBoxItem).Content.ToString();
                SaveSetting();
            }
        }

        private void selfRatingBtn_CheckChanged(object sender, RoutedEventArgs e)
        {
            if(Session.Settings.IgnoreSelfRatingBool != selfRatingBtn.IsChecked)
            {
                Session.Settings.IgnoreSelfRatingBool = (bool)selfRatingBtn.IsChecked;
                SaveSetting();
            }
        }


        private void emailReportBtn_CheckChanged(object sender, RoutedEventArgs e)
        {
            if (Session.Settings.EmailReportBool != emailReportBtn.IsChecked)
            {
                Session.Settings.EmailReportBool = (bool)emailReportBtn.IsChecked;
                SaveSetting();
            }
        }
    }
}
