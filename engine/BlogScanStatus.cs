using ScraperService.Model;

namespace ScraperService
{
      public class BlogScanStatus
      {
            public string StatusMsg { get; set; }
            public bool IsSuccess { get; set; }
            public DateTime TimeStamp { get; set; }
            public NewTumblBlog Blog { get; set; }

            public BlogScanStatus(string msg, bool isSuccess, NewTumblBlog blog)
            {
                  this.StatusMsg = msg;
                  this.IsSuccess = isSuccess;
                  this.Blog = blog;
                  this.TimeStamp = DateTime.Now;

            }
      }
}