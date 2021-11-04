#nullable disable

namespace ScraperService.Model
{
      using System;
      using System.ComponentModel.DataAnnotations;
      using System.ComponentModel.DataAnnotations.Schema;

      [Microsoft.EntityFrameworkCore.Index(nameof(BlogId), Name = "IX_ImgLinks_BlogId")]
      public partial class ImgLink
      {
            [Key]
            public int ImgLinkId { get; set; }
            public string ImgUrl { get; set; }
            public DateTime? AddedDate { get; set; }
            public int? PostId { get; set; }
            public bool IsDownloaded { get; set; }
            public int BlogId { get; set; }

            [ForeignKey(nameof(BlogId))]
            [InverseProperty(nameof(TumBlog.ImgLinks))]
            public virtual TumBlog Blog { get; set; }
      }
}