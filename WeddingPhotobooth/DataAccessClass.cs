using System;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Xml.Serialization;

namespace EZ_http_browser
{
    public class DataAccessClass
    {
        public event AsyncCompletedEventHandler downloadCompletedEvent;
        public event DownloadProgressChangedEventHandler downloadProgresChangedEvent;
        public string latestFilePath;
        //public string folder = @"C:\Users\bvtv\source\repos\EZ_http_browser\EZ_http_browser\bin\Debug";
        private Config myConfig;

        public DataAccessClass(Config myConfig)
        {
            this.myConfig = myConfig;
        }

        public void DownloadImage(string url, string saveFileName)
        {
            //int pos = url.LastIndexOf("%5C") + 3;
            //string filename = url.Substring(pos, url.Length - pos);
            System.IO.Directory.CreateDirectory(myConfig.ImageFolder_Default); // Just in case it does not exist.
            latestFilePath = Path.Combine(myConfig.ImageFolder_Default, saveFileName);
            string escaped = Uri.EscapeDataString(url);
            Uri latestImage = new Uri(url);

            using (var client = new WebClient())
            {
                client.DownloadProgressChanged += downloadProgresChangedEvent;
                client.DownloadFileCompleted += downloadCompletedEvent;
                client.DownloadFileAsync(latestImage, latestFilePath);
            }
        }

        internal void CopyLatestImageToFolder(string targetDir)
        {
            System.IO.Directory.CreateDirectory(targetDir); // Just in case it does not exist.
            string latestFilename = Path.GetFileName(latestFilePath);
            string newFile = System.IO.Path.Combine(targetDir, latestFilename);
            System.IO.File.Copy(latestFilePath, newFile, true);
        }

        internal Config GetOrCreateConfig()
        {
            string file = Path.Combine(Directory.GetCurrentDirectory(), "config.xml");
            if (File.Exists(file))
            {
                // Read Config
                using (StreamReader reader = new StreamReader(file))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Config));
                    Config temp = xs.Deserialize(reader) as Config;
                    this.myConfig = temp;
                }
            }
            else
            {
                // Create new config
                myConfig = new Config();
                SaveConfig(myConfig);
            }
            return myConfig;
        }

        public static void SaveConfig(Config theConfig)
        {
            //string exeFile = System.Reflection.Assembly.GetEntryAssembly().Location;
            string dir = Directory.GetCurrentDirectory();
            string file = Path.Combine(Directory.GetCurrentDirectory(), "config.xml");
            using (FileStream fs = File.Create(file))
            {
                XmlSerializer xs = new XmlSerializer(typeof(Config));
                xs.Serialize(fs, theConfig);
            }
        }

        public void ActivateCamera()
        {
            //string portname = "COM9";
            //int baudRate = 9600;

            using (SerialPort ser = new SerialPort(myConfig.SerialComPort, myConfig.SerialBaudRate))
            {
                ser.Open();
                ser.WriteLine("trg");
                ser.Close();
            }
        }
    }
}
