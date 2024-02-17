using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Domain.Entities;

namespace WebBanHang.Infrastructre.Models
{
    public class AppDbContext : DbContext
    {
        // private readonly string _connectionString = "";
        public DbSet<Users> user { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderDetail> order_detail { get; set; }
        public DbSet<OrderStatus> order_status { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<ProductPicture> product_picture { get; set; }
        public DbSet<ProductStatus> product_status { get; set; }
        public DbSet<ProductType> product_type { get; set; }
        public DbSet<ReviewProduct> review_product { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<Payment> payments { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // public AppDbContext(string connectionString) { _connectionString = connectionString; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableDetailedErrors(true);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().OwnsMany(p => p.RefreshTokens,
                a =>
                {
                    a.WithOwner().HasForeignKey("UserID");
                    a.Property<int>("ID");
                    a.HasKey(p => p.ID);
                });
            modelBuilder.Entity<Users>()
                .HasMany(k => k.Reviews).WithOne(x => x.User).HasForeignKey(fk => fk.UserID).IsRequired();
            // Config Order entity relationship
            modelBuilder.Entity<Order>()
                .HasMany(e => e.Details).WithOne(p => p.Order).HasForeignKey(x => x.OrderID).IsRequired();
            modelBuilder.Entity<Users>()
                .HasMany(e => e.Orders).WithOne(p => p.User).HasForeignKey(x => x.UserID).IsRequired();
            modelBuilder.Entity<Order>().
                HasOne(x => x.Payment).WithOne(y => y.Order).HasForeignKey<Order>(k => k.PaymentID).IsRequired();
            modelBuilder.Entity<Order>().
                HasOne(x => x.Status).WithOne(y => y.Order).HasForeignKey<Order>(kx => kx.StatusID).IsRequired();
            // COnfig Product Entity
            modelBuilder.Entity<Product>().Property(x => x.CategoryID).HasColumnName("ProductTypeID");
            modelBuilder.Entity<Product>().
                HasMany(x => x.Picture).WithOne(xz => xz.Product).HasForeignKey(fk => fk.ProductID).IsRequired();
            modelBuilder.Entity<Product>().
                HasOne(x => x.Status).WithOne(x => x.Product).HasForeignKey<Product>(fk => fk.StatusID).IsRequired();
            modelBuilder.Entity<Product>()
                .HasOne(x => x.ProductType).WithOne(y => y.Product).HasForeignKey<Product>(kf => kf.CategoryID).IsRequired();
            modelBuilder.Entity<Product>()
                .HasMany(x => x.ReviewProduct).WithOne(y => y.Product).HasForeignKey(fk => fk.ProductID).IsRequired();
        }
    }
}
