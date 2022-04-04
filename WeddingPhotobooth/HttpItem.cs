namespace EZ_http_browser
{
    public abstract class HttpItem
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsDownloaded { get; set; }
        public bool IsNew { get; set; }
        public string ServerAddress { get; set; }

        //public int NestLevel { get; set; }

        public static HttpItem GetFolderOrFile(string name, string url, string serverAddress) // figures out wheter we have a file or a folder
        {
            if (url.Contains("file="))
            {
                return new HttpImage(name, url);
            }
            if (url.Contains("dir="))
            {
                return new HttpFolder(name, serverAddress, url);
            }
            else
            {
                return null;
            }
        }

        public string GetServerUrl()
        {
            return System.IO.Path.GetPathRoot(this.Url);
            //string first = this.Url.Split(@"/",2)[0];
            //return "";
        }

    }
}
