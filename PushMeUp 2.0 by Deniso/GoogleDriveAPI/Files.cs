using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using System;

namespace PushMeUp_2._0_by_Deniso.GoogleDriveAPI
{
    public static class Files
    {
        public class FilesCreateOptionalParms
        {
            /// Whether to ignore the domain's default visibility settings for the created file. Domain administrators can choose to make all uploaded files visible to the domain by default; this parameter bypasses that behavior for the request. Permissions are still inherited from parent folders.
            public bool? IgnoreDefaultVisibility { get; set; }
            /// Whether to set the 'keepForever' field in the new head revision. This is only applicable to files with binary content in Drive.
            public bool? KeepRevisionForever { get; set; }
            /// A language hint for OCR processing during image import (ISO 639-1 code).
            public string OcrLanguage { get; set; }
            /// Whether the requesting application supports Team Drives.
            public bool? SupportsTeamDrives { get; set; }
            /// Whether to use the uploaded content as indexable text.
            public bool? UseContentAsIndexableText { get; set; }

        }


        public static void Upload(string _filePath, string _fileName, FilesCreateOptionalParms optional = null, string _descrp = "Uploaded by Push Me Up 2.0")
        {
            DriveService service = Oauth2.GetDriveService("credentials.json", "user", new string[] { DriveService.Scope.DriveFile });

            var FileMetaData = new Google.Apis.Drive.v3.Data.File();
            FileMetaData.Name = _fileName;
            FileMetaData.MimeType = GetMimeType(_filePath);

            FilesResource.CreateMediaUpload request;

            Helper.WaitForFile(_filePath);
            using (var stream = new System.IO.FileStream(_filePath, System.IO.FileMode.Open))
            {
                request = service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
                request.Fields = "id";
                request.Upload();
            }

        }

        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString();
            return mimeType;
        }
    }
}
