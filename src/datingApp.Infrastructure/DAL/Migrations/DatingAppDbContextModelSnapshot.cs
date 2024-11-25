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

                    b.ToTable("AccessCodes");

                    NpgsqlEntityTypeBuilderExtensions.IsUnlogged(b, true);
                });

            modelBuilder.Entity("datingApp.Application.DTO.DeletedEntityDto", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("DeletedEntities");
                });

            modelBuilder.Entity("datingApp.Application.DTO.TokenDto", b =>
                {
                    b.Property<string>("Token")
                        .HasColumnType("text");

                    b.Property<DateTime>("ExpirationTime")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Token");

                    b.ToTable("RevokedRefreshTokens");
                });

            modelBuilder.Entity("datingApp.Core.Entities.Match", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("datingApp.Core.Entities.MatchDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDisplayed")
                        .HasColumnType("boolean");

                    b.Property<Guid>("MatchId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("MatchId");

                    b.HasIndex("UserId");

                    b.ToTable("MatchDetail");
                });

            modelBuilder.Entity("datingApp.Core.Entities.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("IsDisplayed")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("MatchDetailId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("MatchId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SendFromId")
                        .HasColumnType("uuid");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("Id");

                    b.HasIndex("CreatedAt");

                    b.HasIndex("MatchDetailId");

                    b.HasIndex("MatchId");

                    b.HasIndex("SendFromId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("datingApp.Core.Entities.Photo", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<int>("Oridinal")
                        .HasColumnType("integer");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("datingApp.Core.Entities.Swipe", b =>
                {
                    b.Property<Guid>("SwipedById")
                        .HasColumnType("uuid");

                    b.Property<Guid>("SwipedWhoId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("Like")
                        .HasColumnType("integer");

                    b.HasKey("SwipedById", "SwipedWhoId");

                    b.HasIndex("SwipedById", "SwipedWhoId", "Like")
                        .IsUnique();

                    b.ToTable("Swipes");
                });

            modelBuilder.Entity("datingApp.Core.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
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

                    b.HasIndex("Sex", "DateOfBirth");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("datingApp.Core.Entities.UserSettings", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<int>("PreferredMaxDistance")
                        .HasColumnType("integer");

                    b.Property<int>("PreferredSex")
                        .HasColumnType("integer");

                    b.HasKey("UserId");

                    b.ToTable("UserSettings");
                });

            modelBuilder.Entity("datingApp.Core.Entities.MatchDetail", b =>
                {
                    b.HasOne("datingApp.Core.Entities.Match", null)
                        .WithMany("MatchDetails")
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("datingApp.Core.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("datingApp.Core.Entities.Message", b =>
                {
                    b.HasOne("datingApp.Core.Entities.MatchDetail", null)
                        .WithMany("Messages")
                        .HasForeignKey("MatchDetailId");

                    b.HasOne("datingApp.Core.Entities.Match", null)
                        .WithMany("Messages")
                        .HasForeignKey("MatchId");

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
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("datingApp.Core.Entities.UserSettings", b =>
                {
                    b.HasOne("datingApp.Core.Entities.User", null)
                        .WithOne("Settings")
                        .HasForeignKey("datingApp.Core.Entities.UserSettings", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("datingApp.Core.ValueObjects.Location", "Location", b1 =>
                        {
                            b1.Property<Guid>("UserSettingsUserId")
                                .HasColumnType("uuid");

                            b1.Property<double>("Lat")
                                .HasColumnType("double precision");

                            b1.Property<double>("Lon")
                                .HasColumnType("double precision");

                            b1.HasKey("UserSettingsUserId");

                            b1.ToTable("UserSettings");

                            b1.WithOwner()
                                .HasForeignKey("UserSettingsUserId");
                        });

                    b.OwnsOne("datingApp.Core.ValueObjects.PreferredAge", "PreferredAge", b1 =>
                        {
                            b1.Property<Guid>("UserSettingsUserId")
                                .HasColumnType("uuid");

                            b1.Property<int>("From")
                                .HasColumnType("integer");

                            b1.Property<int>("To")
                                .HasColumnType("integer");

                            b1.HasKey("UserSettingsUserId");

                            b1.ToTable("UserSettings");

                            b1.WithOwner()
                                .HasForeignKey("UserSettingsUserId");
                        });

                    b.Navigation("Location");

                    b.Navigation("PreferredAge");
                });

            modelBuilder.Entity("datingApp.Core.Entities.Match", b =>
                {
                    b.Navigation("MatchDetails");

                    b.Navigation("Messages");
                });

            modelBuilder.Entity("datingApp.Core.Entities.MatchDetail", b =>
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
