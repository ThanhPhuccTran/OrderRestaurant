using Microsoft.EntityFrameworkCore;

namespace OrderRestaurant.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        public DbSet<Category> Categoies { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Table> Tables { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetails>()
            .HasKey(ba => new { ba.OrderId,ba.FoodId });

            modelBuilder.Entity<OrderDetails>()
                .HasOne(ba => ba.Order)
                .WithMany(b => b.OrderDetails)
                .HasForeignKey(ba => ba.OrderId);

            modelBuilder.Entity<OrderDetails>()
                .HasOne(ba => ba.Food)
                .WithMany(a => a.OrderDetails)
                .HasForeignKey(ba => ba.FoodId);
        } 
    }
}
