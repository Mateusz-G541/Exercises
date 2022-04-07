using Microsoft.EntityFrameworkCore;

namespace RestaurantAPI.Entities
{
    public class RestaurantDBContext : DbContext
    {
        private string _connectionString = "Server = WDZLAP13; Database = RestaurantDb; Trusted_Connection=True";
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Dish> Dishes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Restaurant>()
               .Property(r => r.Name)
               .IsRequired()
               .HasMaxLength(25);

            modelBuilder.Entity<Dish>()
               .Property(d => d.Name)
               .IsRequired();

            modelBuilder.Entity<Address>()
              .Property(a => a.City)
              .IsRequired()
              .HasMaxLength(50);

            modelBuilder.Entity<Address>()
              .Property(a => a.Street)
              .IsRequired()
              .HasMaxLength(50);


        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }


    }
}
