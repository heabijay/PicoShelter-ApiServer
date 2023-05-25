﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PicoShelter_ApiServer.DAL.EF;

namespace PicoShelter_ApiServer.DAL.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    partial class ApplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.17");

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.AccountEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateUTC")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("LastCredentialsChange")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Password")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<string>("Username")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("RoleId");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Accounts");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedDateUTC = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "heabijay@gmail.com",
                            LastCredentialsChange = new DateTime(2023, 5, 24, 21, 0, 0, 0, DateTimeKind.Utc),
                            Password = "$PICOSHELTER$V1$10000$fplVDwM5wZprgIYyiPuF8EPf3H4t52TDCKjK90NkbSDEBaFa",
                            RoleId = 2,
                            Username = "heabijay"
                        });
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.AlbumEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("CreatedDateUTC")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Title")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("UserCode")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("Albums");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.AlbumImageEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AlbumId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateUTC")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("ImageId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AlbumId");

                    b.HasIndex("ImageId");

                    b.ToTable("AlbumImages");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.BanEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AdminId")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("CreatedDateUTC")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("UntilDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AdminId");

                    b.HasIndex("UserId");

                    b.ToTable("Bans");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.ConfirmationEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("AccountId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateUTC")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Data")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Token")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ValidUntilUTC")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Confirmations");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.ImageCommentEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AuthorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateUTC")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("ImageId")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ImageId");

                    b.ToTable("ImageCommentEntity");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.ImageEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateUTC")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DeleteIn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("DeleteJobId")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Extension")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ImageCode")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("tinyint(1)");

                    b.Property<int?>("ProfileId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("ProfileId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.ImageLikeEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateUTC")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("ImageId")
                        .HasColumnType("int");

                    b.Property<int>("ProfileId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ImageId");

                    b.HasIndex("ProfileId");

                    b.ToTable("ImageLikeEntity");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.ProfileAlbumEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AlbumId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateUTC")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("ProfileId")
                        .HasColumnType("int");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AlbumId");

                    b.HasIndex("ProfileId");

                    b.ToTable("ProfileAlbums");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.ProfileEntity", b =>
                {
                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<string>("BackgroundCSS")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("CreatedDateUTC")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Firstname")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Lastname")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("AccountId");

                    b.ToTable("Profiles");

                    b.HasData(
                        new
                        {
                            AccountId = 1,
                            CreatedDateUTC = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Firstname = "Denys",
                            Lastname = "Lesyshak"
                        });
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.ReportEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AuthorId")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("CreatedDateUTC")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("ImageId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ProcessedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("ProcessedById")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ImageId");

                    b.HasIndex("ProcessedById");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.RoleEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDateUTC")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedDateUTC = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "user"
                        },
                        new
                        {
                            Id = 2,
                            CreatedDateUTC = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "admin"
                        });
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.AccountEntity", b =>
                {
                    b.HasOne("PicoShelter_ApiServer.DAL.Entities.RoleEntity", "Role")
                        .WithMany("Accounts")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.AlbumImageEntity", b =>
                {
                    b.HasOne("PicoShelter_ApiServer.DAL.Entities.AlbumEntity", "Album")
                        .WithMany("AlbumImages")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PicoShelter_ApiServer.DAL.Entities.ImageEntity", "Image")
                        .WithMany("AlbumImages")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Album");

                    b.Navigation("Image");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.BanEntity", b =>
                {
                    b.HasOne("PicoShelter_ApiServer.DAL.Entities.ProfileEntity", "Admin")
                        .WithMany("BansProcessed")
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("PicoShelter_ApiServer.DAL.Entities.AccountEntity", "User")
                        .WithMany("Bans")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Admin");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.ConfirmationEntity", b =>
                {
                    b.HasOne("PicoShelter_ApiServer.DAL.Entities.AccountEntity", "Account")
                        .WithMany("Confirmations")
                        .HasForeignKey("AccountId");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.ImageCommentEntity", b =>
                {
                    b.HasOne("PicoShelter_ApiServer.DAL.Entities.ProfileEntity", "Author")
                        .WithMany("ImageComments")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("PicoShelter_ApiServer.DAL.Entities.ImageEntity", "Image")
                        .WithMany("Comments")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Image");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.ImageEntity", b =>
                {
                    b.HasOne("PicoShelter_ApiServer.DAL.Entities.ProfileEntity", "Profile")
                        .WithMany("Images")
                        .HasForeignKey("ProfileId");

                    b.Navigation("Profile");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.ImageLikeEntity", b =>
                {
                    b.HasOne("PicoShelter_ApiServer.DAL.Entities.ImageEntity", "Image")
                        .WithMany("Likes")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PicoShelter_ApiServer.DAL.Entities.ProfileEntity", "Profile")
                        .WithMany()
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Image");

                    b.Navigation("Profile");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.ProfileAlbumEntity", b =>
                {
                    b.HasOne("PicoShelter_ApiServer.DAL.Entities.AlbumEntity", "Album")
                        .WithMany("ProfileAlbums")
                        .HasForeignKey("AlbumId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PicoShelter_ApiServer.DAL.Entities.ProfileEntity", "Profile")
                        .WithMany("ProfileAlbums")
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Album");

                    b.Navigation("Profile");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.ProfileEntity", b =>
                {
                    b.HasOne("PicoShelter_ApiServer.DAL.Entities.AccountEntity", "Account")
                        .WithOne("Profile")
                        .HasForeignKey("PicoShelter_ApiServer.DAL.Entities.ProfileEntity", "AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.ReportEntity", b =>
                {
                    b.HasOne("PicoShelter_ApiServer.DAL.Entities.ProfileEntity", "Author")
                        .WithMany("Reports")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PicoShelter_ApiServer.DAL.Entities.ImageEntity", "Image")
                        .WithMany("Reports")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PicoShelter_ApiServer.DAL.Entities.ProfileEntity", "ProcessedBy")
                        .WithMany("ReportsProcessed")
                        .HasForeignKey("ProcessedById");

                    b.Navigation("Author");

                    b.Navigation("Image");

                    b.Navigation("ProcessedBy");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.AccountEntity", b =>
                {
                    b.Navigation("Bans");

                    b.Navigation("Confirmations");

                    b.Navigation("Profile");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.AlbumEntity", b =>
                {
                    b.Navigation("AlbumImages");

                    b.Navigation("ProfileAlbums");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.ImageEntity", b =>
                {
                    b.Navigation("AlbumImages");

                    b.Navigation("Comments");

                    b.Navigation("Likes");

                    b.Navigation("Reports");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.ProfileEntity", b =>
                {
                    b.Navigation("BansProcessed");

                    b.Navigation("ImageComments");

                    b.Navigation("Images");

                    b.Navigation("ProfileAlbums");

                    b.Navigation("Reports");

                    b.Navigation("ReportsProcessed");
                });

            modelBuilder.Entity("PicoShelter_ApiServer.DAL.Entities.RoleEntity", b =>
                {
                    b.Navigation("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}
