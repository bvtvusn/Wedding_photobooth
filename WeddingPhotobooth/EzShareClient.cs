using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EZ_http_browser
{
    public class EzShareClient
    {
        public Timer PollTimer { get; set; }
        //public delegate void MyDel(string str);
        public event Action<List<HttpItem>> PhotoDiscoveredEvent;

        public event Action<DateTime> SuccessfulPollEvent;
        public event Action<string> ErrorEvent;
        //public string RootFolder { get; set; }
        //List<IHtmlAnchorElement> imageList;
        bool firstSearchFlag = true;
        List<HttpItem> httpItemList;
        HttpFolder root;


        public EzShareClient(string rootFolder)
        {
            PollTimer = new Timer();
            PollTimer.Interval = 1000;
            PollTimer.Enabled = false;
            PollTimer.Tick += PollTimer_Tick;

            //RootFolder = "http://ezshare.card/dir?dir=A:%5CDCIM%5C100NCD40";

            //imageList = new List<IHtmlAnchorElement>();
            httpItemList = new List<HttpItem>();
            root = new HttpFolder("root", "", rootFolder);
        }

        private async void PollTimer_Tick(object sender, EventArgs e)
        {
            SearchForNewElements();

        }
        public bool PingTest()
        {
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();

            // Use the default Ttl value which is 128,
            // but change the fragmentation behavior.
            options.DontFragment = true;

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120;
            Uri myUri = new Uri(root.Url);
            string host = myUri.Host;

            try
            {
                PingReply reply = pingSender.Send(host, timeout, buffer, options);
            }
            catch (Exception err)
            {

                return false;
            }
            return true;


        }
        public async void SearchForNewElements()
        {
            bool connected = PingTest();
            if (!connected)
            {
                //throw new Exception();
                ErrorEvent("Camera not found on network");
            }
            else
            {
                Task task = root.UpdateTreeAsync(0);

                int timeout = 1000;
                //var task = SomeOperationAsync();
                if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
                {
                    // task completed within timeout
                    SuccessfulPollEvent?.Invoke(DateTime.Now);
                    //ErrorEvent("2test");

                    List<HttpItem> newItems = FindNewItems_rec(root);
                    if (newItems.Count > 0 && firstSearchFlag == false)
                    {
                        PhotoDiscoveredEvent(newItems);
                    }
                    firstSearchFlag = false;
                }
                else
                {
                    ErrorEvent?.Invoke("Poll timeout");
                    // Maybe we have to stop the thread as well?
                }
            }


            //await t;


        }

        List<HttpItem> FindNewItems_rec(HttpFolder folder)
        {
            List<HttpItem> newItems = new List<HttpItem>();
            foreach (HttpItem item in folder.Children)
            {
                if (item.IsNew)
                {
                    item.IsNew = false;
                    newItems.Add(item);
                }
                if (item is HttpFolder)
                {
                    newItems.AddRange(FindNewItems_rec(item as HttpFolder));
                }
            }
            return newItems;
        }
    }
}
