namespace ScraperService
{
      using System.Diagnostics;


      using HtmlAgilityPack;

      using OpenQA.Selenium.Edge;

      using ScraperService.Data;
      using ScraperService.Model;



      public class PageScraper : BrowserControl, IDisposable
      {

            private bool disposedValue1;
            private readonly ILogger _logger;

            // private readonly PageScraper _instance;

            //
            //
            //#######################

            public EdgeDriver TheDriver { get; set; }
            private IDataController _dataController;

            public PageScraper(ILogger logger, IDataController dataController)
            {
                  _logger = logger;
                  _dataController = dataController;
                  this.TheDriver ??= InitializeWebDriver().Result;

            }




            public async Task BeginSiteScrapeAsync(CancellationToken stoppingToken)
            {

                  List<TumBlog>? list = _dataController.GetBlogList(5, 5);
                  if (list.Count == 0)
                  {
                        Worker.TurnTimerOff();
                  }
                  try
                  {
                        foreach (TumBlog blob in list)
                        {
                              await LoadBlogForScraping(blob, stoppingToken);

                        }
                        Console.WriteLine("Batch is complete....");
                  }
                  catch (TaskCanceledException ex)
                  {
                        _logger
                        }
                  finally
                  {
                        this.TheDriver.Quit();
                  }
            }

            internal async Task LoadBlogForScraping(TumBlog blog, CancellationToken stoppingToken)
            {
                        HtmlDocument document = new();
                  try
                  {
                        // WebDriverWait wait = new WebDriverWait(this.TheDriver, TimeSpan.FromSeconds(30)).Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a")));
                        this.TheDriver.Navigate().GoToUrl($"{blog.BlogUrl}#t=7");
                        await Task.Delay(5000, stoppingToken);

                        _ = this.TheDriver.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
                        await Task.Delay(10000, stoppingToken);
                        _ = this.TheDriver.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
                        await Task.Delay(15000, stoppingToken);


                        document.DisableServerSideCode = true;
                        document.LoadHtml(this.TheDriver.PageSource);
                  }
                  catch (OpenQA.Selenium.WebDriverException wde)
                  {
                        _logger.PageScraperNavigationFault(wde);
                  }
                  if (!stoppingToken.IsCancellationRequested)
                  {
                        try
                        {
                              //perform the page scraping
                              Console.WriteLine("starting to scan {0}...", blog.BlogName);
                              if (ScrapeVideoNodes(document, blog.BlogId).IsFaulted)
                              {
                                    Console.WriteLine("Scraper vids returned error");
                                    _dataController.UpdateScanStatus(new BlogScanStatus("Failed", false, blog));
                              }
                              else
                              {

                                    //Scraping is complete lets update the database of our status
                                    _dataController.UpdateScanStatus(new BlogScanStatus("Success", true, blog));
                                    Console.WriteLine($"Page scrape complete... ");

                              }

                        }
                        catch (HtmlAgilityPack.NodeNotFoundException)
                        {
                              using IDisposable? scope = _logger.ProcessingWorkScope(DateTime.Now);
                              _logger.PageScraperSiteCaptureFault();
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
                  using LinkDBContext db = new();
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


            */








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
      }


























      public class BlogScanStatus
      {
            public string StatusMsg { get; set; }
            public bool IsSuccess { get; set; }
            public DateTime TimeStamp { get; set; }
            public TumBlog Blog { get; set; }

            public BlogScanStatus(string msg, bool isSuccess, TumBlog blog)
            {
                  this.StatusMsg = msg;
                  this.IsSuccess = isSuccess;
                  this.Blog = blog;
                  this.TimeStamp = DateTime.Now;

            }
      }



}