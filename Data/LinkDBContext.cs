#nullable disable

namespace ScraperService.Data
{
    using System;

    using Microsoft.EntityFrameworkCore;

    using ScraperService.Model;

    public partial class LinkDBContext : DbContext
    {


        private readonly ILogger _logger;


        public LinkDBContext(ILogger logger)
        {
            _logger = logger;
            this.SaveChangesFailed += OnFailedSavedChanges;
        }
public LinkDBContext()
{
      
}

        private void OnFailedSavedChanges(object sender, SaveChangesFailedEventArgs e)
        {

            _logger.DatabaseUpdateFault();
        }

        public virtual DbSet<ImgLink> ImgLinks { get; set; }
        public virtual DbSet<NewTumblBlog> NewTumblBlogs { get; set; }
        public virtual DbSet<VidLink> VidLinks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder is null)
            {
                throw new ArgumentNullException(nameof(optionsBuilder));
            }

            if (!optionsBuilder.IsConfigured)
            {
                _ = optionsBuilder.UseSqlServer("server=Localhost;user=sa;password=Cubby2022;database=ScraperBlogs;");
            }


        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            _ = modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");


            OnModelCreatingPartial(modelBuilder);

        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


        // modelBuilder.Entity<TumBlog>().MapToStoredProcedures;


    }






}