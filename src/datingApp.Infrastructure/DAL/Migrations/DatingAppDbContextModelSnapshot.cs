﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using datingApp.Infrastructure;

#nullable disable

namespace datingApp.Infrastructure.DAL.Migrations
{
    [DbContext(typeof(DatingAppDbContext))]
    partial class DatingAppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("datingApp.Application.DTO.AccessCodeDto", b =>
                {
                    b.Property<string>("EmailOrPhone")
                        .HasColumnType("text");

                    b.Property<string>("AccessCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("ExpirationTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<TimeSpan>("Expiry")
                        .HasColumnType("interval");

                    b.HasKey("EmailOrPhone");

                    b.HasIndex("EmailOrPhone");

                    b.ToTable("AccessCodes");
                });

            modelBuilder.Entity("datingApp.Application.DTO.FileDto", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<byte[]>("Binary")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("Extension")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("datingApp.Core.Entities.Match", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsDisplayedByUser1")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsDisplayedByUser2")
                        .HasColumnType("boolean");

                    b.Property<Guid>("UserId1")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId2")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId2");

                    b.HasIndex("UserId1", "UserId2", "CreatedAt");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("datingApp.Core.Entities.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsDisplayed")
                        .HasColumnType("boolean");

                    b.Property<Guid>("MatchId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SendFromId")
                        .HasColumnType("uuid");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("Id");

                    b.HasIndex("SendFromId");

                    b.HasIndex("MatchId", "CreatedAt");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("datingApp.Core.Entities.Photo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Oridinal")
                        .HasColumnType("integer");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId", "Oridinal");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("datingApp.Core.Entities.Swipe", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("Like")
                        .HasColumnType("integer");

                    b.Property<Guid>("SwipedById")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SwipedWhoId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("SwipedById", "SwipedWhoId")
                        .IsUnique();

                    b.HasIndex("SwipedById", "SwipedWhoId", "Like");

                    b.ToTable("Swipes");
                });

            modelBuilder.Entity("datingApp.Core.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Bio")
                        .HasMaxLength(400)
                        .HasColumnType("character varying(400)");

                    b.Property<DateOnly>("DateOfBirth")
                        .HasColumnType("date");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("Job")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("character varying(15)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(9)
                        .HasColumnType("character varying(9)");

                    b.Property<int>("Sex")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Phone")
                        .IsUnique();

                    b.HasIndex("Sex", "DateOfBirth");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("datingApp.Core.Entities.UserSettings", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<int>("DiscoverAgeFrom")
                        .HasColumnType("integer");

                    b.Property<int>("DiscoverAgeTo")
                        .HasColumnType("integer");

                    b.Property<int>("DiscoverRange")
                        .HasColumnType("integer");

                    b.Property<int>("DiscoverSex")
                        .HasColumnType("integer");

                    b.Property<double>("Lat")
                        .HasColumnType("double precision");

                    b.Property<double>("Lon")
                        .HasColumnType("double precision");

                    b.HasKey("UserId");

                    b.HasIndex("Lat", "Lon");

                    b.ToTable("UserSettings");
                });

            modelBuilder.Entity("datingApp.Application.DTO.FileDto", b =>
                {
                    b.HasOne("datingApp.Core.Entities.Photo", null)
                        .WithOne()
                        .HasForeignKey("datingApp.Application.DTO.FileDto", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("datingApp.Core.Entities.Match", b =>
                {
                    b.HasOne("datingApp.Core.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("datingApp.Core.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId2")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("datingApp.Core.Entities.Message", b =>
                {
                    b.HasOne("datingApp.Core.Entities.Match", null)
                        .WithMany("Messages")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("datingApp.Core.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("SendFromId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("datingApp.Core.Entities.Photo", b =>
                {
                    b.HasOne("datingApp.Core.Entities.User", null)
                        .WithMany("Photos")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("datingApp.Core.Entities.UserSettings", b =>
                {
                    b.HasOne("datingApp.Core.Entities.User", null)
                        .WithOne("Settings")
                        .HasForeignKey("datingApp.Core.Entities.UserSettings", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("datingApp.Core.Entities.Match", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("datingApp.Core.Entities.User", b =>
                {
                    b.Navigation("Photos");

                    b.Navigation("Settings");
                });
#pragma warning restore 612, 618
        }
    }
}
