#nullable disable

namespace ScraperService.Model
{
      using System.ComponentModel.DataAnnotations;
      using System.ComponentModel.DataAnnotations.Schema;

      [Microsoft.EntityFrameworkCore.Index(nameof(BlogId), Name = "IX_VidLinks_BlogId")]
      public partial class VidLink
      {
            [Key]
            public int VidLinkId { get; set; }
            public string VidUrl { get; set; }
            public DateTime? AddedDate { get; set; }
            public int? PostId { get; set; }
            public bool IsDownloaded { get; set; }
            public int BlogId { get; set; }

            [ForeignKey(nameof(BlogId))]
            [InverseProperty(nameof(TumBlog.VidLinks))]

            [Required]
            public virtual TumBlog Blog { get; set; }
      }
}