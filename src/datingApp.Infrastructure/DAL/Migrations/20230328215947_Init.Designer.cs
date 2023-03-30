﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using datingApp.Infrastructure;

#nullable disable

namespace datingApp.Infrastructure.DAL.Migrations
{
    [DbContext(typeof(DatingAppDbContext))]
    [Migration("20230328215947_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("datingApp.Core.Entities.Match", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("UserId1")
                        .HasColumnType("integer");

                    b.Property<int>("UserId2")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("datingApp.Core.Entities.Message", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsDisplayed")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false);

                    b.Property<int>("MatchId")
                        .HasColumnType("integer");

                    b.Property<int>("SendFromId")
                        .HasColumnType("integer");

                    b.Property<int>("SendToId")
                        .HasColumnType("integer");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("Id");

                    b.HasIndex("MatchId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("datingApp.Core.Entities.Photo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Oridinal")
                        .HasColumnType("integer");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("datingApp.Core.Entities.Swipe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("Like")
                        .HasColumnType("integer");

                    b.Property<int>("SwippedById")
                        .HasColumnType("integer");

                    b.Property<int>("SwippedWhoId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Swipes");
                });

            modelBuilder.Entity("datingApp.Core.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

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
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)");

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

                    b.ToTable("Users");
                });

            modelBuilder.Entity("datingApp.Core.Entities.UserSettings", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

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

                    b.ToTable("UserSettings");
                });

            modelBuilder.Entity("datingApp.Core.Entities.Message", b =>
                {
                    b.HasOne("datingApp.Core.Entities.Match", null)
                        .WithMany("Messages")
                        .HasForeignKey("MatchId")
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
