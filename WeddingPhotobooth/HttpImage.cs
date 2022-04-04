using System;

namespace EZ_http_browser
{
    public class HttpImage : HttpItem
    {
        public HttpImage(string name, string url)
        {
            Name = name;
            Url = url;
        }

        public string ThumbnailURL
        {
            get
            {
                //string str = this.Url;
                ////string[] parameters = str.Split('?');
                //string output = parameters[0];


                //string[] separator = new string[] { "%5C" };
                ////str = str.Replace(@"/download?", "/thumbnail?");
                ////str = str.Replace(@"&folderFlag = 0", "&ftype=0");
                //return output;
                return CreateThumbnailLink(this.Url);
            }
        }
        public static string CreateThumbnailLink(string str)
        {

            string[] SplitRes1 = str.Split('?');
            string pureUrl = SplitRes1[0];
            string allParams = SplitRes1[1];

            string[] SplitRes2 = allParams.Split('=');
            string imgData = SplitRes2[1];


            string[] separator = new string[] { "%5C" };
            string[] imgDataparts = imgData.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            string filename = imgDataparts[imgDataparts.Length - 1];
            string folder = imgDataparts[imgDataparts.Length - 2];

            string result = pureUrl.Replace(@"/download", @"/thumbnail");
            result += "?fname=" + filename + "&fdir=" + folder + "&ftype=0";
            //str = str.Replace(@"/download?", "/thumbnail?");
            //str = str.Replace(@"&folderFlag = 0", "&ftype=0");
            return result;
        }


    }
}
