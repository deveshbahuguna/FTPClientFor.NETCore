using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace FTPFileTransfer
{
    class FTPHelper
    {
        public static string FtpServerIp = string.Empty;
        public static string Username = string.Empty;
        public static string Password = string.Empty;


        public static void CreateFTPDirectory(string directoryName)
        {
            WebRequest ftpRequest = WebRequest.Create("ftp://" + FtpServerIp+"/"+directoryName);
            ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
            ftpRequest.Credentials = new NetworkCredential(Username, Password);
            ftpRequest.GetResponse();
        }
        

        public static void DownloadFile(string fileNameWithPath)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + FtpServerIp+"//"+fileNameWithPath);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(Username, Password);
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            string fileName = fileNameWithPath;
            var file = File.Create(fileName);
            responseStream.CopyTo(file);
            file.Close();
            Console.WriteLine($"Download Complete, status {response.StatusDescription}");
            response.Close();
        }

        public static void DownloadDirectory(string directoryNameWithPath)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + FtpServerIp + "/" + directoryNameWithPath);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            request.Credentials = new NetworkCredential(Username, Password);
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string directoryName = directoryNameWithPath;
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            ////{f|d} <file or directory name absolute path>
            ////f for file and d for directory
            string fileTypeAndFileName = reader.ReadLine();
            while (fileTypeAndFileName != null)
            {
                string[] fileDetails = fileTypeAndFileName.Split(' ');

                ////Check if this is directory or file
                if (fileDetails[0].Contains('d'))
                {
                    DownloadDirectory(directoryNameWithPath + "/" + fileDetails[fileDetails.Length - 1]);
                }
                else
                {
                    DownloadFile(directoryNameWithPath + "/" + fileDetails[fileDetails.Length - 1]);
                }
                fileTypeAndFileName = reader.ReadLine();
            }
        }

        public static void UploadFile(string fileName)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://" + FtpServerIp+"/"+fileName);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(Username, Password);
            byte[] uploadContents;
            uploadContents = File.ReadAllBytes(fileName);
            request.ContentLength = uploadContents.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(uploadContents, 0, uploadContents.Length);
            }

            using (FtpWebResponse resp = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine($"Upload File Complete, status {resp.StatusDescription}");
            }
        }

        public static void UploadDirectory(string directoryName)
        {
            CreateFTPDirectory(directoryName);
            DirectoryInfo directory = new DirectoryInfo(directoryName);
            foreach (FileInfo file in directory.GetFiles())
            {
                UploadFile(directoryName+ "/" + file.Name);
            }
            foreach (var sudDirectory in directory.GetDirectories())
            {
                UploadDirectory(directoryName + "/" + sudDirectory.Name);
            }
        }
    }
}
