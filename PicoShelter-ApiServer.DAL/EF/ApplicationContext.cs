using Microsoft.EntityFrameworkCore;
using PicoShelter_ApiServer.DAL.Entities;
using System;

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
        public DbSet<ReportEntity> Reports { get; set; }
        public DbSet<BanEntity> Bans { get; set; }


        private readonly string _connectionString;

        public ApplicationContext(string connectionString) : base()
        {
            _connectionString = connectionString;
            //Database.EnsureDeleted();
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


            AccountEntity adminAccount = new()
            {
                Id = 1,
                Email = "heabijay@gmail.com",
                Username = "heabijay",
                Password = "$PICOSHELTER$V1$10000$fplVDwM5wZprgIYyiPuF8EPf3H4t52TDCKjK90NkbSDEBaFa",
                RoleId = 2,
                LastCredentialsChange = DateTime.Today,
            };

            ProfileEntity adminProfile = new()
            {
                Firstname = "Denys",
                Lastname = "Lesyshak",
                AccountId = 1
            };

            builder.Entity<AccountEntity>().HasData(adminAccount);
            builder.Entity<ProfileEntity>().HasData(adminProfile);


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

            builder.Entity<ReportEntity>()
                .HasOne(t => t.Image)
                .WithMany(t => t.Reports)
                .HasForeignKey(t => t.ImageId);

            builder.Entity<ReportEntity>()
                .HasOne(t => t.Author)
                .WithMany(t => t.Reports)
                .HasForeignKey(t => t.AuthorId);

            builder.Entity<ReportEntity>()
                .HasOne(t => t.ProcessedBy)
                .WithMany(t => t.ReportsProcessed)
                .HasForeignKey(t => t.ProcessedById);

            builder.Entity<BanEntity>()
                .HasOne(t => t.User)
                .WithMany(t => t.Bans)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<BanEntity>()
                .HasOne(t => t.Admin)
                .WithMany(t => t.BansProcessed)
                .HasForeignKey(t => t.AdminId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
