using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PushMeUp_2._0_by_Deniso.DropboxAPI
{
    class DropBoxBase
    {
        #region Variables  
        private DropboxClient DBClient;
        private ListFolderArg DBFolders;
        private string oauth2State;
        private const string RedirectUri = "https://localhost/authorize"; // Same as we have configured Under [Application] -> settings -> redirect URIs.  
        #endregion

        #region Constructor  
        public DropBoxBase(string ApiKey, string ApiSecret, string ApplicationName = "Push Me Up 2.0")
        {
            try
            {
                AppKey = ApiKey;
                AppSecret = ApiSecret;
                AppName = ApplicationName;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
        #region Properties  
        public string AppName
        {
            get; private set;
        }
        public string AuthenticationURL
        {
            get; private set;
        }
        public string AppKey
        {
            get; private set;
        }

        public string AppSecret
        {
            get; private set;
        }

        public string AccessTocken
        {
            get; private set;
        }
        public string Uid
        {
            get; private set;
        }
        #endregion

        #region UserDefined Methods  

        /// <summary>  
        /// This method is to generate Authentication URL to redirect user for login process in Dropbox.  
        /// </summary>  
        /// <returns></returns>  
        public string GeneratedAuthenticationURL()
        {
            try
            {
                this.oauth2State = Guid.NewGuid().ToString("N");
                Uri authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, AppKey, RedirectUri, state: oauth2State);
                AuthenticationURL = authorizeUri.AbsoluteUri.ToString();
                return authorizeUri.AbsoluteUri.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>  
        /// This method is to generate Access Token required to access dropbox outside of the environment (in ANy application).  
        /// </summary>  
        /// <returns></returns>  
        public string GenerateAccessToken()
        {
            try
            {
                string _strAccessToken = string.Empty;

                if (CanAuthenticate())
                {
                    if (string.IsNullOrEmpty(AuthenticationURL))
                    {
                        throw new Exception("AuthenticationURL is not generated !");

                    }
                    LoginDropbox login = new LoginDropbox(AppKey, AuthenticationURL, this.oauth2State); // WPF window with Webbrowser control to redirect user for Dropbox login process.  
                    login.Owner = Application.Current.MainWindow;
                    login.ShowDialog();
                    if (login.Result)
                    {
                        _strAccessToken = login.AccessToken;
                        AccessTocken = login.AccessToken;
                        Uid = login.Uid;
                        DropboxClientConfig CC = new DropboxClientConfig(AppName, 1);
                        HttpClient HTC = new HttpClient();
                        HTC.Timeout = TimeSpan.FromMinutes(10); // set timeout for each ghttp request to Dropbox API.  
                        CC.HttpClient = HTC;
                        DBClient = new DropboxClient(AccessTocken, CC);
                    }
                    else
                    {
                        DBClient = null;
                        AccessTocken = string.Empty;
                        Uid = string.Empty;
                    }
                }

                return _strAccessToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>  
        /// Method to create new folder on Dropbox  
        /// </summary>  
        /// <param name="path"> path of the folder we want to create on Dropbox</param>  
        /// <returns></returns>  
        public bool CreateFolder(string path)
        {
            try
            {
                if (AccessTocken == null)
                {
                    throw new Exception("AccessToken not generated !");
                }
                if (AuthenticationURL == null)
                {
                    throw new Exception("AuthenticationURI not generated !");
                }

                var folderArg = new CreateFolderArg(path);
                var folder = DBClient.Files.CreateFolderAsync(folderArg);
                var result = folder.Result;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        /// <summary>  
        /// Method is to check that whether folder exists on Dropbox or not.  
        /// </summary>  
        /// <param name="path"> Path of the folder we want to check for existance.</param>  
        /// <returns></returns>  
        public bool FolderExists(string path)
        {
            try
            {
                if (AccessTocken == null)
                {
                    throw new Exception("AccessToken not generated !");
                }
                if (AuthenticationURL == null)
                {
                    throw new Exception("AuthenticationURI not generated !");
                }

                var folders = DBClient.Files.ListFolderAsync(path);
                var result = folders.Result;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        
        /// <summary>  
        /// Method to upload files on Dropbox  
        /// </summary>  
        /// <param name="UploadfolderPath"> Dropbox path where we want to upload files</param>  
        /// <param name="UploadfileName"> File name to be created in Dropbox</param>  
        /// <param name="SourceFilePath"> Local file path which we want to upload</param>  
        /// <returns></returns>  
        public bool Upload(string UploadfolderPath, string UploadfileName, string SourceFilePath)
        {
            try
            {
                using (var stream = new MemoryStream(File.ReadAllBytes(SourceFilePath)))
                {
                    var response = DBClient.Files.UploadAsync(UploadfolderPath + "/" + UploadfileName, WriteMode.Overwrite.Instance, body: stream);
                    var rest = response.Result; //Added to wait for the result from Async method  
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        #endregion
        #region Validation Methods  
        /// <summary>  
        /// Validation method to verify that AppKey and AppSecret is not blank.  
        /// Mendatory to complete Authentication process successfully.  
        /// </summary>  
        /// <returns></returns>  
        public bool CanAuthenticate()
        {
            try
            {
                if (AppKey == null)
                {
                    throw new ArgumentNullException("AppKey");
                }
                if (AppSecret == null)
                {
                    throw new ArgumentNullException("AppSecret");
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion
    }
}
