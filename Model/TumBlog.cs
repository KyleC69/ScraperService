#nullable disable

namespace ScraperService.Model
{
      using System.ComponentModel.DataAnnotations;
      using System.ComponentModel.DataAnnotations.Schema;

      public partial class TumBlog
      {
            public TumBlog()
            {
                  this.ImgLinks = new HashSet<ImgLink>();
                  this.VidLinks = new HashSet<VidLink>();
            }
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            [Key]
            public int BlogId { get; set; }
            public string BlogName { get; set; }
            public string BlogUrl { get; set; }
            public DateTime? LastScanDate { get; set; }
            public string LastScanStatus { get; set; }
            public int? LastScannedPostId { get; set; }
            public bool? IsEnabled { get; set; }

            [InverseProperty(nameof(ImgLink.Blog))]
            public virtual ICollection<ImgLink> ImgLinks { get; set; }
            [InverseProperty(nameof(VidLink.Blog))]
            public virtual ICollection<VidLink> VidLinks { get; set; }
      }
}