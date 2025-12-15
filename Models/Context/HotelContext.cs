using Hotel.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Models.Context
{
    public class HotelContext : IdentityDbContext<User>
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomPhoto> RoomPhotos { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingItem> BookingItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        public HotelContext(DbContextOptions<HotelContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Identity table customization
            builder.Entity<User>().ToTable("AspNetUsers");

            // Configure decimal precision for Room.Price
            builder.Entity<Room>()
                .Property(r => r.Price)
                .HasPrecision(18, 2);

            // Configure decimal precision for BookingItem.PricePerNight
            builder.Entity<BookingItem>()
                .Property(bi => bi.PricePerNight)
                .HasPrecision(18, 2);

            // Configure relationships
            builder.Entity<BookingItem>()
                .HasOne(bi => bi.Booking)
                .WithMany(b => b.BookingItems)
                .HasForeignKey(bi => bi.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BookingItem>()
                .HasOne(bi => bi.Room)
                .WithMany()
                .HasForeignKey(bi => bi.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}