// "Copyright 2021 (c) Kyle Crowder, Lawrence Enterprises. All Rights Reserved - Use may be granted with License. "

namespace ScraperService.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.EntityFrameworkCore;

    using ScraperService.Model;


    public interface IDataController
    {

        List<NewTumblBlog> GetBlogList(int daysFromLastScan);
        List<NewTumblBlog> GetBlogList(int daysFromLastScan, int Qty);
        void UpdateImageLink(ImgLink img);
        void UpdateScanStatus(BlogScanStatus status);
        void UpdateScanStatus(NewTumblBlog itm);
        void UpdateVideoLink(string vidLink);
        void UpdateVideoLink(VidLink vid);
    }

    public class DataController : IDisposable, IDataController
    {

        private ILogger _logger;



        public DataController(ILogger logger)
        {
            _logger = logger;
        }

        public IEnumerable<string> TestUrlList { get => testUrlList; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DataController()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }



        public Task AddNewTumblBlog(NewBlog blog)
        {
            NewTumblBlog bl = new NewTumblBlog { BlogId = blog.BlogId, BlogName = blog.BlogName, BlogEnabled = true, BlogUrl = blog.BlogUrl };
            bl.InsertedDate = DateTime.Now;

            using LinkDBContext db = new();
            {
                if(db.NewTumblBlogs.Any(m => m.BlogId == bl.BlogId)){return Task.CompletedTask;}
                try
                {
                    db.NewTumblBlogs.Add(bl);
                    db.SaveChanges();
                    return Task.CompletedTask;
                }
                catch
                {
                    return Task.FromException(new DatabaseUpdateException());
                }
            }

        }

        public Task AddNewTumblBlog(NewTumblBlog blog)
        {
            using LinkDBContext db = new();
            {
                if (db.NewTumblBlogs.Any(m => m.BlogId == blog.BlogId)) { return Task.CompletedTask; }
                try
                {
                    db.NewTumblBlogs.Add(blog);
                    db.SaveChanges();
                    return Task.CompletedTask;
                }
                catch
                {
                    return Task.FromException(new DatabaseUpdateException());
                }
            }
        }










        public void UpdateVideoLink(string vidLink)
        {

            using LinkDBContext db = new(_logger);
            VidLink vid = db.VidLinks.Select(x => x)
            .Where(y => y.VidUrl.Equals(vidLink)).Single();

            vid.IsDownloaded = true;

            _ = db.Update(vid);
            _ = db.SaveChanges();
        }


        public void UpdateVideoLink(VidLink vid)
        {
            using LinkDBContext db = new(_logger);
            vid.IsDownloaded = true;
            _ = db.Update(vid);
            _ = db.SaveChanges();
        }

        public void UpdateImageLink(ImgLink img)
        {
            // throw new NotImplementedException();
        }

        private readonly IEnumerable<string> testUrlList = new string[]
         {
    "https://dn1.newtumbl.com/img/306319/10054449/1/14860134/nT_jsks0p81ga9ax89gk2u6r8c2.mp4",
    "https://dn0.newtumbl.com/img/940506/85013336/1/645196/nT_dz0t13y5u9sgf63xqgqksr16.mp4",
    "https://dn3.newtumbl.com/img/558728/37538067/1/54790073/nT_4beddn2bnryasn5hr91afkth.mp4",
    "https://dn3.newtumbl.com/img/14858/73665999/1/105416202/nT_46t7czksejyjai803nieiyp8.mp4",
    "https://dn1.newtumbl.com/img/16580/13533097/1/19533221/nT_48ncngsxjby2vyiq2f112403.mp4"
         };
        private bool disposedValue;

        /*

                internal static List<VidLink> GetVideoLinkObjects(int qty)
                {
                    using LinkDBContext db = new();
                    List<VidLink> q = (from x in db.VidLinks.Include(c => c.Blog)
                    .Where(b => b.IsDownloaded == false)
                                       select x).OrderBy(g => Guid.NewGuid()).Take(qty).ToList();
                    return q;

                }


                internal  Task<VidLink> GetVidLinkByFileName(string v)
                {
                    using LinkDBContext db = new();
                    VidLink q = (from x in db.VidLinks.Where(b => b.VidUrl.Contains(v)) select x).SingleOrDefault();
                    return Task.FromResult(q);

                }




                internal void InsertNewBlog(NewTumblBlog NewtumblBlog)
                {
                    try
                    {
                        using LinkDBContext db = new();
                        SqlParameter blogid = new("@BlogID", NewtumblBlog.BlogId);
                        SqlParameter blogname = new("@BlogName", NewtumblBlog.BlogName);
                        SqlParameter blogurl = new("@BlogUrl", NewtumblBlog.BlogUrl);

                        string cmd = $"exec InsertNewNewTumblBlog(@blogID={NewtumblBlog.BlogId},@BlogName={NewtumblBlog.BlogName},@BlogUrl={NewtumblBlog.BlogUrl})";
                        db.Database.ExecuteSqlRaw("exec InsertNewNewTumblblog @blogId, @BlogName, @BlogUrl", blogid, blogname, blogurl);
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine(ex);
                    }
                    finally
                    {

                    }

                }




                internal  Task<NewTumblBlog> GetBlogIdByFileName(string filename)
                {
                    NewTumblBlog p = new();
                    using LinkDBContext db = new();
                    VidLink query = (from x in db.VidLinks.Where(b => b.VidUrl.Contains(filename)) select x).Single();

                    if (query != null)
                    {
                        p = (from q in db.NewTumblBlogs.Where(c => c.BlogId == query.BlogId) select q).SingleOrDefault();
                    }
                    return Task.FromResult(p);

                }




                internal  void ListBlogIDs()
                {
                    using LinkDBContext db = new();
                    var q = (from x in db.NewTumblBlogs
                             select new { x.BlogId, x.BlogName }).ToList();
                    foreach (var c in q)
                    {
                        Console.WriteLine($"ID: {c.BlogId} Name: {c.BlogName}");
                    };"textMateRules": [
            {
                "scope": "keyword.operator",
                "settings": {
                    "foreground": "#a200ff"
                }
                }

        */


        /// <summary>
        /// Gets List of all active blog links that have
        /// not been scanned since <paramref name="daysFromLastScan"/>
        /// </summary>
        /// <param name="daysFromLastScan"></param>
        /// <returns>List<NewTumblBlog></returns>
        public List<NewTumblBlog> GetBlogList(int daysFromLastScan)
        {
            using LinkDBContext? db = new(_logger);

            List<NewTumblBlog> blog = (from x in db.NewTumblBlogs
                                       where x.LastScanDate <= DateTime.Now.AddDays(daysFromLastScan)
                                       && (x.BlogEnabled == true)
                                       select x).ToList();

            return blog;
        }

        /// <summary>
        /// Returns Blog ready for scanning
        /// </summary>
        /// <param name="daysFromLastScan">Number of days from last successful scan</param>
        /// <param name="Qty">Number of Blogs to return</param>
        /// <returns>List<Blog></returns>
        public List<NewTumblBlog> GetBlogList(int daysFromLastScan, int Qty)
        {
            List<NewTumblBlog> bloglist = new();
            using LinkDBContext? db = new(_logger);
            try
            {
                bloglist = (from x in db.NewTumblBlogs
                            where x.LastScanDate <= DateTime.Now.AddDays(daysFromLastScan - (daysFromLastScan * 2))
                                          && (x.BlogEnabled == true || x.LastScanDate == null)
                            select x).Take(Qty).ToList();


                //    blog = (from x in db.NewTumblBlogs where x.IsEnabled == true && (x.LastScanDate == null) select x).Take(Qty).ToList();


            }
            catch
            {
                _logger.DatabaseQueryFault();

            }
            return bloglist;
        }




        public void UpdateScanStatus(BlogScanStatus status)
        {
            using LinkDBContext db = new(_logger);
            try
            {
                NewTumblBlog itm = status.Blog;
                itm.LastScanDate = status.TimeStamp;
                itm.LastScanStatus = status.StatusMsg;
                _ = db.Update(itm);
                _ = db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                Console.WriteLine("Failed to update Database... Concurrency exc.");
            }
            catch (DbUpdateException)
            {

                Console.WriteLine("Failed to update DB Update Exception");
            }
            catch (Exception)
            {
                Console.WriteLine("General database failure.");
            }
            finally
            {
                db.Dispose();
            }
        }

        public void UpdateScanStatus(NewTumblBlog itm)
        {
            try
            {
                using LinkDBContext db = new(_logger);
                itm.LastScanDate = DateTime.Now;
                itm.LastScanStatus = " Success";
                _ = db.Update(itm);
                _ = db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                Console.WriteLine("DB fail");
            }
            catch (DbUpdateException)
            {
                Console.WriteLine("DB fail");
            }
            finally
            {
            }
        }








        internal List<VidLink> GetVidLinksByBlogID(int BlogID)
        {
            using LinkDBContext db = new(_logger);
            List<VidLink> q = (from x in db.VidLinks.Include(c => c.Blog)
            .Where(b => b.IsDownloaded == false && b.BlogId == BlogID)
                               select x).OrderBy(g => Guid.NewGuid()).ToList();
            return q;
        }

        internal void UpdateVideoLink(Uri site)
        {
            using LinkDBContext db = new(_logger);
            VidLink vid = db.VidLinks.Select(x => x)
            .Where(y => y.VidUrl.Equals(site.ToString())).Single();

            vid.IsDownloaded = true;

            _ = db.Update(vid);
            _ = db.SaveChanges();
        }


        internal int GetTotalImgLinks()
        {
            using LinkDBContext db = new(_logger);
            int cnt = (from x in db.ImgLinks where x.IsDownloaded == false select x).Count();
            return cnt;
        }

        internal int GetTotalVidLinks()
        {
            using LinkDBContext db = new(_logger);
            int cnt = (from x in db.VidLinks select x).Count();
            return cnt;
        }

        internal int GetAvailableToDownload()
        {
            using LinkDBContext db = new(_logger);
            int cnt = (from x in db.VidLinks where x.IsDownloaded == false select x).Count();
            return cnt;
        }

        internal int GetActiveBlogCount()
        {
            using LinkDBContext db = new(_logger);
            int cnt = (from x in db.NewTumblBlogs where x.BlogEnabled == true select x).Count();
            return cnt;

        }

        internal void PrintDatabaseStats() => Console.WriteLine($"To Dl: {GetAvailableToDownload()}.  Total Vid: {GetTotalVidLinks()}.   Total Img: {GetTotalImgLinks()}.  ActiveBlogs:  {GetActiveBlogCount()}", ConsoleColor.Cyan);
    }

    public static class DataBaseStats
    {

        public static int AvailableToDownload { get; }
        public static int TotalVidLinkRecords { get; }
        public static int TotalImageLinkRecords { get; }
        public static int ActiveBlogs { get; }


    }


}