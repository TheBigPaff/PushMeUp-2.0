using Dropbox.Api;
using Google.Apis.Drive.v3;
using PushMeUp_2._0_by_Deniso.GoogleDriveAPI;
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
using PushMeUp_2._0_by_Deniso.DropboxAPI;

namespace PushMeUp_2._0_by_Deniso
{
    enum Service
    {
        GoogleDrive,
        Dropbox,
        OneDrive,
        Email
    }

    /// <summary>
    /// Interaction logic for BackupControl.xaml
    /// </summary>
    public partial class BackupControl : UserControl
    {
        Service serviceUsed;

        #region Variables
        private string strAccessToken = string.Empty;
        private string strAuthenticationURL = string.Empty;
        private DropBoxBase DBB;
        #endregion


        string backupName { get
            {
                return InputTextBox.Text + ".db";
            } }

        const string databasePath = "PushMeUpDB.db";
        public BackupControl()
        {
            InitializeComponent();

            InitializeBackupExplanation();

            //string toolTip = $"Because of the costs of running servers, this app was made with a local database (SQLite) in mind.\nOne drawback of using local databases though is that you could lose all your data if the local machine gets lost or destroyed." +
            //    $"\nThat's why one feature of 'PushMeUp 2.0' is backing up your data on the cloud, using your favorite file storage service or by sending you an email with your data." +
            //    $"\n\nSaving your data is easy, just click on one of the buttons and follow the instructions. To actually restore your data though, download the database from the cloud and put it in the root folder of 'PushMeUp2.0.exe'";
            //toolTipTb.Text = toolTip;
        }

        private void InitializeBackupExplanation()
        {
            backupExplanationTb.Inlines.Clear();
            backupExplanationTb.Inlines.Add(new Run("Because of the costs of running servers, this app was made with a local database (SQLite) in mind.\nOne drawback of using local databases though is that you could lose all your data if the local machine gets lost or destroyed." +
                $"\nThat's why one feature of"));
            backupExplanationTb.Inlines.Add(new Run(" 'PushMeUp 2.0'") { FontStyle = FontStyles.Italic });
            backupExplanationTb.Inlines.Add(new Run(" is backing up your data on the cloud, using your favorite file storage service or by sending you an email with your data."));

            backupExplanationTb.Inlines.Add(new Run("\n\nSaving your data") { Foreground = Brushes.Teal });
            backupExplanationTb.Inlines.Add(new Run(" is easy, just click on one of the buttons and follow the instructions.\nTo"));

            backupExplanationTb.Inlines.Add(new Run(" restore your data") { Foreground = Brushes.Teal });
            backupExplanationTb.Inlines.Add(new Run(", download the database from the cloud and put it in the root folder of"));

            backupExplanationTb.Inlines.Add(new Run(" \'PushMeUp2.0.exe'") { FontStyle = FontStyles.Italic });
        }

        private void tipBtn_Click(object sender, RoutedEventArgs e)
        {
            if(backupExplanationBox.Visibility == Visibility.Visible)
            {
                backupExplanationBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                backupExplanationBox.Visibility = Visibility.Visible;
            }
        }

        public void BackupCard_Click(object sender, RoutedEventArgs e)
        {
            switch(((sender as Button).Parent as BackupCard).BackupName)
            {
                case "Google Drive":
                    serviceUsed = Service.GoogleDrive;
                    InputBox.Visibility = Visibility.Visible;
                    break;
                case "Dropbox":
                    serviceUsed = Service.Dropbox;
                    //InputBox.Visibility = Visibility.Visible;
                    serviceNotAvailableGrid.Visibility = Visibility.Visible;
                    break;
                case "OneDrive":
                    serviceUsed = Service.OneDrive;
                    serviceNotAvailableGrid.Visibility = Visibility.Visible;
                    break;
                case "Email":
                    serviceUsed = Service.Email;
                    InputBox.Visibility = Visibility.Visible;
                    break;
            }
            
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            // validate input
            if (String.IsNullOrWhiteSpace(InputTextBox.Text))
            {
                MessageBox.Show("You need to insert a file name.");
                return;
            }
            if (InputTextBox.Text.Length > 16)
            {
                MessageBox.Show("File name can't be longer than 16 characters.");
                return;
            }

            switch (serviceUsed)
            {
                case Service.GoogleDrive:
                    string _backupName = backupName;
                    Task.Factory.StartNew(() => GoogleDriveAPI.Files.Upload(databasePath, _backupName));
                    break;
                case Service.Dropbox:
                    UploadDropbox();
                        break;
                case Service.Email:
                    SendEmail();
                    break;
            }

            InputBox.Visibility = Visibility.Collapsed;
        }

        private void SendEmail()
        {
            if (string.IsNullOrWhiteSpace(Session.LoggedUser.Email))
            {
                MessageBox.Show("You don't have an email. Set one first in the 'User Info' tab");
                return;
            }

            // make message
            string dateAndTimeString = DateTime.Now.ToShortDateString() + " | " + DateTime.Now.ToShortTimeString();
            string title = $"Database Backup - PushMeUp 2.0";
            string body = $"Hello {Session.LoggedUser.Username},\nAttached to this email there's the backup of your data you requested.\nSent on {dateAndTimeString}.\n\n- Push Me Up 2.0 #DOTHETHING";


            string _backupName = backupName;
            // email user on a separate thread
            Task.Factory.StartNew(() => Helper.SendEmail(Session.LoggedUser.Email, title, body, databasePath, _backupName));
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            InputBox.Visibility = Visibility.Collapsed;
        }

        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            serviceNotAvailableGrid.Visibility = Visibility.Collapsed;
        }


        private void UploadDropbox()
        {
            try
            {
                Authenticate();
                if (DBB != null)
                {
                    if (strAccessToken != null && strAuthenticationURL != null)
                    {
                        DBB.Upload("/Dropbox/PushMeUp", backupName , @"PushMeUpDB.db");
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        
        #region Private Methods    
        public void Authenticate()
        {
            try
            {
                //if (string.IsNullOrEmpty(strAppKey))
                //{
                //    MessageBox.Show("Please enter valid App Key !");
                //    return;
                //}
                if (DBB == null)
                {
                    DBB = new DropBoxBase(Helper.LoadConnectionString("Dropbox_API_Key"), "PushMeUp");

                    strAuthenticationURL = DBB.GeneratedAuthenticationURL(); // This method must be executed before generating Access Token.    
                    strAccessToken = DBB.GenerateAccessToken();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
