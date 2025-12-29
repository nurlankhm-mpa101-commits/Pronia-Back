using Microsoft.EntityFrameworkCore;
using Pronia.Models;

namespace Pronia.Contexts;

public class ProniaDbContext: DbContext
{
    public DbSet<Card> Cards { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    public DbSet<Product> Products { get; set; }
    
    public DbSet<Tag> Tags { get; set; }
    
    public DbSet<ProductTag> ProductTags { get; set; }


    public ProniaDbContext(DbContextOptions<ProniaDbContext> options): base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductTag>()
            .HasKey(pt => new { pt.ProductId, pt.TagId });

        modelBuilder.Entity<ProductTag>()
            .HasOne(pt => pt.Product)
            .WithMany(p => p.ProductTags)
            .HasForeignKey(pt => pt.ProductId);

        modelBuilder.Entity<ProductTag>()
            .HasOne(pt => pt.Tag)
            .WithMany(t => t.ProductTags)
            .HasForeignKey(pt => pt.TagId);

        base.OnModelCreating(modelBuilder);
    }

}

