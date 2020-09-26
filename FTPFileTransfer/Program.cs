using System;
using System.IO;
using System.Net;
using System.Text;

namespace FTPFileTransfer
{
    class Program
    {
             
        static void Main(string[] args)
        {
            Console.WriteLine("Enter FTPServerIp");
            FTPHelper.FtpServerIp = Console.ReadLine();
            Console.WriteLine("Enter UserName_Password");
            string[] credentials = Console.ReadLine().Split('_');
            FTPHelper.Username = credentials[0];
            FTPHelper.Password = credentials[1];
            while (true)
            {
                Console.WriteLine("Press U for upload or D for download or E for exit");
                string operation = Console.ReadLine();
                Console.WriteLine("Enter 'f(file) fileName' or 'd(directory) directoryName'");
                string[] fileDetails = Console.ReadLine().Split(' ');

                switch (operation.ToUpper())
                {
                    case "U":
                        {
                            if (fileDetails[0].ToUpper() == "F")
                            {
                                FTPHelper.UploadFile(fileDetails[1]);
                            }
                            else
                            {
                                FTPHelper.UploadDirectory(fileDetails[1]);
                            }
                            break;
                        }
                    case "D":
                        {
                            if (fileDetails[0].ToUpper() == "F")
                            {
                                FTPHelper.DownloadFile(fileDetails[1]);
                            }
                            else
                            {
                                FTPHelper.DownloadDirectory(fileDetails[1]);
                            }
                            break;
                        }
                    default: return;
                }
                
            }
        }
    }
}
