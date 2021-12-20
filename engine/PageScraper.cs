namespace ScraperService
{
    using System.Diagnostics;


    using HtmlAgilityPack;

    using OpenQA.Selenium.Edge;

    using ScraperService.Data;
    using ScraperService.Model;



    public interface IPageScraper
    {

        Task BeginSiteScrapeAsync(CancellationToken stoppingToken);
    }






    public class PageScraper : BrowserControl, IPageScraper, IDisposable
    {

        private EdgeDriver? _edgeDriver;
        private bool disposedValue;

        //#######################



        public PageScraper(ILogger logger)
        {
            base.Logger = logger;
        }


        public static Task BeginScan(ILogger logger, CancellationToken stoppingToken)
        {
            TaskCompletionSource tcs = new();

            _ = Task.Run(async () =>
              {

                  PageScraper ps = new(logger);
                  try
                  {
                      await ps.BeginSiteScrapeAsync(stoppingToken);
                      tcs.SetResult();

                  }
                  catch (System.Exception ex)
                  {
                      logger.PageScraperUnknownFault(ex);
                      tcs.SetException(ex);
                  }
                  finally
                  {
                      ps.Dispose();
                  }
              }, stoppingToken);




            return tcs.Task;
        }
        public async Task BeginSiteScrapeAsync(CancellationToken stoppingToken)
        {
            using DataController dc = new(Logger);
            List<NewTumblBlog>? list = dc.GetBlogList(5, 50);

            if (list.Count == 0)
            {
                Worker.TurnTimerOff();
                return;// Task.CompletedTask;
            }
            if (TheDriver.Equals(null))
            { Logger.WebDriverInitializationFault(); }
            if (!base.IsLoggedIn) { await DoSiteLogin(); }



            try
            {
                foreach (NewTumblBlog blob in list)
                {
                    await LoadBlogForScraping(blob, stoppingToken);
                }
                Console.WriteLine("Batch is complete....");
                Environment.Exit(0);
            }
            catch (TaskCanceledException)
            {
                this.Logger.TaskCanceledException();
            }
            catch (Exception ex)
            {
                this.Logger.PageScraperUnknownFault(ex);
            }
            finally
            {
                this.TheDriver.Quit();
            }
        }
        internal void PageScroll(int iterations)
        {
            for (int x = 0; x <= iterations; x++)
            {

                this.TheDriver.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
                Task.Delay(5000).Wait();
            }

        }
        internal async Task LoadBlogForScraping(NewTumblBlog blog, CancellationToken stoppingToken)
        {
            HtmlDocument document = new();
            try
            {
                // WebDriverWait wait = new WebDriverWait(this.TheDriver, TimeSpan.FromSeconds(30)).Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a")));


                this.TheDriver.Navigate().GoToUrl($"{blog.BlogUrl}#t=7");
                await Task.Delay(5000, stoppingToken);
// Set number of pages to capture. Count = 50 per page
                PageScroll(2);

                document.DisableServerSideCode = true;
                document.LoadHtml(this.TheDriver.PageSource);
            }
            catch (OpenQA.Selenium.WebDriverException wde)
            {
                Logger.PageScraperNavigationFault(wde);
            }
            catch (Exception ex)
            {
                  Logger.PageScraperUnknownFault(ex);
            }

            if (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    //perform the page scraping
                    Console.WriteLine("starting to scan {0}...", blog.BlogName);
                    using (DataController dc = new(Logger))
                    {
                        if (ScrapeVideoNodes(document, blog.BlogId).IsFaulted)
                        {
                            Console.WriteLine("Scraper vids returned error");
                            dc.UpdateScanStatus(new BlogScanStatus("Failed", false, blog));
                        }
                        else
                        {

                            //Scraping is complete lets update the database of our status
                            dc.UpdateScanStatus(new BlogScanStatus("Success", true, blog));
                            Console.WriteLine($"Page scrape complete... ");

                        }
                    }

                }
                catch (HtmlAgilityPack.NodeNotFoundException)
                {
                    using IDisposable? scope = Logger.ProcessingWorkScope(DateTime.Now);
                    Logger.PageScraperSiteCaptureFault();
                }
            }
        }



        public Task ScrapeVideoNodes(HtmlDocument doc, int BlogId)
        {
            int vidCount;
            if (Equals(doc, null)) { throw new Exception("html doc is null"); }
            // Console.WriteLine.LogDebug("Separating image and video nodes");
            // HtmlNodeCollection PostNodes = doc.DocumentNode.SelectNodes("//div[@class='block']");

            //           var vids = PostNodes.SelectMany(v1 => v1.SelectNodes("[descendant::video]"));
            //           var imgs = PostNodes.SelectMany(v2 => v2.SelectNodes("//img[1][not(@delay_src)]"));
            HtmlNodeCollection VideoNodes = doc.DocumentNode.SelectNodes("//div[@class='block'][descendant::video]");
            // HtmlNodeCollection test = doc.DocumentNode.SelectNodes("//div[@class='NTPOST___post nt_post post'][descendant::video]");
            //  HtmlNodeCollection ImageNodes = null; //doc.DocumentNode.SelectNodes("//div[@class='block'][@blog!='0'][descendant::img]");
            //         Debugger.Break();
            if (VideoNodes != null)
            {
                vidCount = VideoNodes.Count;
                VidLink theVid;
                foreach (HtmlNode vid in VideoNodes)
                {
                    theVid = new();
                    theVid.BlogId = BlogId;
                    theVid.PostId = Convert.ToInt32(vid.GetAttributeValue("post", null));
                    theVid.VidUrl = vid.SelectSingleNode("descendant::video/source").GetAttributeValue("src", "");
                    theVid.AddedDate = DateTime.Now;
                    //.FirstChild.GetAttributeValue("src", null);
                    //    QueuedHostedService.AddUrlToQueAsync(theVid);
                    AddNewVidLinks(theVid);

                    // AsyncDownloader dl = new(AsyncDownloader.Client);
                    // QueuedHostedService.QuickQue.Enqueue(() => DownloadFileTaskAsync(new Uri(theVid.VidUrl), AsyncDownloader.BuildSavePath(theVid)));
                }
                Console.WriteLine($"    Captured.. {vidCount} Video Links");
            }
            else
            {
                return Task.FromException(new Exception("No video nodes during scrape"));
            }
            return Task.CompletedTask;
        }




        private void AddNewVidLinks(VidLink newVid)
        {
            using LinkDBContext db = new(Logger);
            if (!db.VidLinks.Any(m => m.VidUrl == newVid.VidUrl))
            {
                try
                {
                    newVid.AddedDate = DateTime.Now;

                    _ = db.VidLinks.Add(newVid);
                    _ = db.SaveChanges();

                }
                catch
                {
                    Debug.Fail("DB save failed");
                }
                finally
                {
                    db?.Dispose();
                }
            }

            //if (!db.VidAssets.Any(u => u.VidUrl == blogObj.BlogUrl))
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    TheDriver.Quit();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PageScraper()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}



/*
        internal static void ParsePageAsync()
        {
            try
            {
                HtmlDocument doc = new();
                doc.LoadHtml(TheDriver.PageSource);
                // HtmlNode outernode = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'NTCOMMON___postArrangeBody')]");
                var nodes = doc.DocumentNode.SelectNodes("//div[@class='blog_marquee')]");
                /*
                                var nav = outernode.CreateNavigator();
                                nav.MoveToFirstChild();
                                ScrapeBlogChildNode(((HtmlNodeNavigator)nav).CurrentNode.ChildNodes[1]);
                                while (nav.MoveToNext())
                                {
                                    ScrapeBlogChildNode(((HtmlNodeNavigator)nav).CurrentNode.ChildNodes[1]);
                                }

                foreach (HtmlNode node in nodes)
                {
                    ScrapeBlogChildNode(node);
                }
                blogLists.ForEach(b =>
                {
                    DataController.InsertNewBlog(b);
                });
            }
            catch (System.Exception)
            {

            }
        }











internal void ScrapeBlogChildNode(HtmlNode node)
{
      TumBlog blog = new();

      //#### Get the descendant node of currentyly selected node with an attribute of blog
      HtmlNode? bname = node.SelectSingleNode("//div[@class='blog_name']");
      string? txt = bname.InnerText;

      string? burl = $"https://{txt}.newtumbl.com";

      HtmlNode? iconsrc = node.SelectSingleNode("//div[@class='blog_icon']/img");
      string? src = iconsrc.GetAttributeValue("src", "");
      Uri? u = new(src);
      blog.BlogId = Convert.ToInt32(u.Segments [ 2 ].Remove(u.Segments [ 2 ].Length - 1, 1));
      blog.BlogName = txt;
      blog.BlogUrl = burl;

      // blogLists.Add(blog);

}

protected virtual void Dispose(bool disposing)
{
      if (!disposedValue1)
      {
            if (disposing)
            {
                  // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue1 = true;
      }
}

// // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources


public void Dispose()
{
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
}


*/