using System.IO;

namespace EZ_http_browser
{
    public class Config
    {
        public string ImageFolder_Default { get; set; }
        public string ImageFolder_Keep { get; set; }
        public string ImageFolder_Deleted { get; set; }
        public string SerialComPort { get; set; }
        public int SerialBaudRate { get; set; }


        public Config()
        {
            string basePath = Path.GetDirectoryName( System.Reflection.Assembly.GetEntryAssembly().Location);
            ImageFolder_Default = Path.Combine(basePath,"Default");
            ImageFolder_Deleted = Path.Combine(basePath,"Delete");
            ImageFolder_Keep = Path.Combine(basePath, "Keep");

            //ImageFolder_Default = @"C:\Users\bvtv\source\repos\EZ_http_browser\EZ_http_browser\bin\Debug\Default";
            //ImageFolder_Deleted = @"C:\Users\bvtv\source\repos\EZ_http_browser\EZ_http_browser\bin\Debug\Delete";
            //ImageFolder_Keep = @"C:\Users\bvtv\source\repos\EZ_http_browser\EZ_http_browser\bin\Debug\Keep";
            SerialComPort = "COM9";
            SerialBaudRate = 9600;
        }
    }
}