using Microsoft.EntityFrameworkCore;
using PicoShelter_ApiServer.DAL.Entities;

namespace PicoShelter_ApiServer.DAL.EF
{
    public class ApplicationContext : DbContext
    {
        public DbSet<AccountEntity> Accounts { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<ProfileEntity> Profiles { get; set; }
        public DbSet<ImageEntity> Images { get; set; }
        public DbSet<AlbumEntity> Albums { get; set; }
        public DbSet<ProfileAlbumEntity> ProfileAlbums { get; set; }
        public DbSet<AlbumImageEntity> AlbumImages { get; set; }
        public DbSet<ConfirmationEntity> Confirmations { get; set; }


        private string _connectionString;

        public ApplicationContext(string connectionString) : base()
        {
            _connectionString = connectionString;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.UseLazyLoadingProxies();

            var isMySQL = _connectionString.Contains(";Uid=", System.StringComparison.OrdinalIgnoreCase) || _connectionString.StartsWith("Uid=", System.StringComparison.OrdinalIgnoreCase);

            if (isMySQL)
                builder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
            else
                builder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            RoleEntity userRole = new() { Id = 1, Name = "user" };
            RoleEntity adminRole = new() { Id = 2, Name = "admin" };

            builder.Entity<RoleEntity>().HasData(userRole, adminRole);

            builder.Entity<ProfileAlbumEntity>()
                .HasOne(t => t.Profile)
                .WithMany(t => t.ProfileAlbums)
                .HasForeignKey(t => t.ProfileId);

            builder.Entity<ProfileAlbumEntity>()
                .HasOne(t => t.Album)
                .WithMany(t => t.ProfileAlbums)
                .HasForeignKey(t => t.AlbumId);


            builder.Entity<AlbumImageEntity>()
                .HasOne(t => t.Album)
                .WithMany(t => t.AlbumImages)
                .HasForeignKey(t => t.AlbumId);

            builder.Entity<AlbumImageEntity>()
                .HasOne(t => t.Image)
                .WithMany(t => t.AlbumImages)
                .HasForeignKey(t => t.ImageId);


            builder.Entity<AccountEntity>()
                .HasIndex(t => t.Username)
                .IsUnique();

            builder.Entity<AccountEntity>()
                .HasIndex(t => t.Email)
                .IsUnique();

            builder.Entity<AccountEntity>()
                .Property(t => t.RoleId)
                .HasDefaultValue(1);
        }
    }
}
