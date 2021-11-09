using ScraperService.Model;

namespace ScraperService
{
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