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
        public DbSet<ManageStatus> Statuss { get; set; }
        public DbSet<Requirements> Requests { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Permission> Permissions { get; set; }

       // public DbSet<Cart> CartUser { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permission>().HasNoKey();
            modelBuilder.Entity<OrderDetails>()
            .HasKey(ba => new { ba.OrderId, ba.FoodId });

            modelBuilder.Entity<OrderDetails>()
                .HasOne(ba => ba.Order)
                .WithMany(b => b.OrderDetails)
                .HasForeignKey(ba => ba.OrderId);

            modelBuilder.Entity<OrderDetails>()
                .HasOne(ba => ba.Food)
                .WithMany(a => a.OrderDetails)
                .HasForeignKey(ba => ba.FoodId);

            modelBuilder.Entity<Order>()
        .HasOne(o => o.Tables)
        .WithMany(t => t.Orders)
        .HasForeignKey(o => o.TableId)
        .OnDelete(DeleteBehavior.Restrict);



        /*    modelBuilder.Entity<Cart>()
            .HasOne(c => c.TableCart)
            .WithMany(t => t.Carts)
            .HasForeignKey(c => c.TableId)
            .OnDelete(DeleteBehavior.NoAction); // Hoặc sử dụng DeleteBehavior.Restrict nếu bạn muốn

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.FoodCart)
                .WithMany(f => f.Carts)
                .HasForeignKey(c => c.FoodId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.EmployeeCart)
                .WithMany(e => e.Carts)
                .HasForeignKey(c => c.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.ManageStatusCart)
                .WithMany(e => e.Carts)
                .HasForeignKey(c => c.StatusId)
                .OnDelete(DeleteBehavior.NoAction);
*/
           
        }
    }
}
