using AngleSharp;
using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EZ_http_browser
{
    public class HttpFolder : HttpItem
    {
        public HttpFolder(string name, string serverAddress, string relPath)
        {
            this.Name = name;
            this.ServerAddress = serverAddress;
            Children = new List<HttpItem>();
            this.Url = relPath;
        }

        public async Task UpdateTreeAsync(int nestLevel)
        {
            if (nestLevel == 10)
            {
                return; // avoid infinite stacks
            }

            // Fill in all the children

            Task t = UpdateChildrenAsync();
            await t;

            // Reqursively call each of the children:
            var tasks = Children.Where(obj => obj is HttpFolder).Select(i => (i as HttpFolder).UpdateTreeAsync(nestLevel + 1));

            await Task.WhenAll(tasks);

        }
        public List<HttpItem> Children { get; set; }

        public string GetFullPath()
        {
            if (Url.Length > 0)
            {
                string full = ServerAddress + "/" + Url; // The urls on folders are always relative on EZ share card.
                return full;
            }
            else
            {
                return ServerAddress;
            }
        }

        public async Task UpdateChildrenAsync()
        {

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            AngleSharp.Dom.IDocument document;

            int timeout = 1000;
            var task = context.OpenAsync(Url); // important line
            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
                // task completed within timeout
                document = task.Result;
            }
            else
            {
                // timeout logic
                throw new Exception();
            }

            //try
            //{
            //    document = await context.OpenAsync(Url);
            //}
            //catch (Exception)
            //{

            //    throw new Exception();
            //}


            //task.R
            //var document = await context.OpenAsync(Url);
            var cellSelector = "pre a";
            var cells = document.QuerySelectorAll(cellSelector);
            //throw new Exception();

            IHtmlAnchorElement[] linkstrings = cells.OfType<IHtmlAnchorElement>().ToArray();

            List<HttpItem> discoveries = new List<HttpItem>();

            for (int i = 0; i < linkstrings.Length; i++)
            {
                string href = linkstrings[i].Href;
                int count = Children.Where(p => p.Url == href).Count(); // does it already exist? How many?

                if (count == 0 && linkstrings[i].InnerHtml != " .." && linkstrings[i].InnerHtml != " .") // Add new link
                {
                    HttpItem discovered = HttpItem.GetFolderOrFile(linkstrings[i].InnerHtml, linkstrings[i].Href, ServerAddress);
                    discovered.IsNew = true;
                    discoveries.Add(discovered);
                }
            }
            Children.AddRange(discoveries);

        }
    }
}
