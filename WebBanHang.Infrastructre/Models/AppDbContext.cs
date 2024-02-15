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
        private readonly string _connectionString = "";
        public DbSet<Users> user { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderDetail> order_detail { get; set; }
        public DbSet<OrderStatus> order_status { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<ProductPicture> product_picture { get; set; }
        public DbSet<ProductStatus> product_status { get; set; }
        public DbSet<ProductType> product_type { get; set; }
        public DbSet<ReviewDetail> review_detail { get; set; }
        public DbSet<ReviewHub> review_hub { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<Payment> payments { get; set; }
        public DbSet<StoredToken> stored_token { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public AppDbContext(string connectionString) { _connectionString = connectionString; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(_connectionString);
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
        }
    }
}
