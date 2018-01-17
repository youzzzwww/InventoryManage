using InventoryManage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace InventoryManage.Data
{
    public class CenterContext : IdentityDbContext<Worker, CenterRole, int>
    {
        public CenterContext(DbContextOptions<CenterContext> options) : base(options)
        { }

        public DbSet<Worker> Workers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Equipment> Equipments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Worker>().ToTable("Worker");
            modelBuilder.Entity<Invoice>().ToTable("Invoice");
            modelBuilder.Entity<Invoice>().HasOne(i => i.Worker).WithMany(w => w.Invoices).HasForeignKey(i => i.WorkerId);
            modelBuilder.Entity<Equipment>().ToTable("Equipment");
        }
    }
}
