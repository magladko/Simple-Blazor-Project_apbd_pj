using APBDproject.Server.Models;
using APBDproject.Shared.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APBDproject.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Daily> Daily { get; set; }
        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Symbol);
                entity.Property(e => e.Name).IsRequired();
                entity.HasMany(e => e.Users).WithMany(e => e.Companies);
                entity.HasOne(e => e.Daily).WithOne(e => e.Company);
                entity.HasMany(e => e.Articles).WithMany(e => e.Companies);
            });

            builder.Entity<Daily>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Symbol).IsRequired();
                entity.Property(e => e.Open).IsRequired();
                entity.Property(e => e.High).IsRequired();
                entity.Property(e => e.Low).IsRequired();
                entity.Property(e => e.Close).IsRequired();
                entity.Property(e => e.Volume).IsRequired();
                entity.Property(e => e.AfterHours).IsRequired();
                entity.Property(e => e.PreMarket).IsRequired();
                entity.HasOne(e => e.Company).WithOne(e => e.Daily).HasForeignKey<Daily>(e => e.Symbol);
            });

            builder.Entity<Article>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Author).IsRequired();
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.PublishedUtc).IsRequired();
                entity.Property(e => e.ArticleUrl).IsRequired();
                entity.HasMany(e => e.Companies).WithMany(e => e.Articles);
            });
        }
    }
}
