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

            private void OnFailedSavedChanges(object sender, SaveChangesFailedEventArgs e)
            {

                  _logger.DatabaseUpdateFault();
            }

            public virtual DbSet<ImgLink> ImgLinks { get; set; }
            public virtual DbSet<TumBlog> TumBlogs { get; set; }
            public virtual DbSet<VidLink> VidLinks { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                  if (optionsBuilder is null)
                  {
                        throw new ArgumentNullException(nameof(optionsBuilder));
                  }

                  if (!optionsBuilder.IsConfigured)
                  {
                        _ = optionsBuilder.UseSqlServer("server=172.17.0.2;user=sa;password=Angel1031;database=LinkDB;");
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