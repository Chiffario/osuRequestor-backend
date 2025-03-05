﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using osuRequestor.Data;

#nullable disable

namespace osuRequestor.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20250305081004_InitMigration")]
    partial class InitMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("osuRequestor.Models.BeatmapModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<double>("ApproachRate")
                        .HasColumnType("double precision");

                    b.Property<int>("BeatmapSetId")
                        .HasColumnType("integer");

                    b.Property<double>("BeatsPerMinute")
                        .HasColumnType("double precision");

                    b.Property<double>("CircleSize")
                        .HasColumnType("double precision");

                    b.Property<int>("Circles")
                        .HasColumnType("integer");

                    b.Property<double>("HealthDrain")
                        .HasColumnType("double precision");

                    b.Property<int>("MaxCombo")
                        .HasColumnType("integer");

                    b.Property<int>("Mode")
                        .HasColumnType("integer");

                    b.Property<double>("OverallDifficulty")
                        .HasColumnType("double precision");

                    b.Property<int>("Sliders")
                        .HasColumnType("integer");

                    b.Property<int>("Spinners")
                        .HasColumnType("integer");

                    b.Property<double>("StarRating")
                        .HasColumnType("double precision");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("BeatmapSetId");

                    b.ToTable("Beatmaps");
                });

            modelBuilder.Entity("osuRequestor.Models.BeatmapSetModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Artist")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("CreatorId")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("BeatmapSets");
                });

            modelBuilder.Entity("osuRequestor.Models.RequestModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BeatmapId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Date")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("now()");

                    b.Property<int>("RequestedFromId")
                        .HasColumnType("integer");

                    b.Property<int>("RequestedToId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("BeatmapId");

                    b.HasIndex("RequestedFromId");

                    b.HasIndex("RequestedToId");

                    b.ToTable("Requests");
                });

            modelBuilder.Entity("osuRequestor.Models.TokenModel", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<string>("AccessToken")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("UserId");

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("osuRequestor.Models.UserModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AvatarUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CountryCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("osuRequestor.Models.BeatmapModel", b =>
                {
                    b.HasOne("osuRequestor.Models.BeatmapSetModel", "BeatmapSet")
                        .WithMany()
                        .HasForeignKey("BeatmapSetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BeatmapSet");
                });

            modelBuilder.Entity("osuRequestor.Models.RequestModel", b =>
                {
                    b.HasOne("osuRequestor.Models.BeatmapModel", "Beatmap")
                        .WithMany()
                        .HasForeignKey("BeatmapId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("osuRequestor.Models.UserModel", "RequestedFrom")
                        .WithMany()
                        .HasForeignKey("RequestedFromId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("osuRequestor.Models.UserModel", "RequestedTo")
                        .WithMany()
                        .HasForeignKey("RequestedToId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Beatmap");

                    b.Navigation("RequestedFrom");

                    b.Navigation("RequestedTo");
                });

            modelBuilder.Entity("osuRequestor.Models.TokenModel", b =>
                {
                    b.HasOne("osuRequestor.Models.UserModel", "User")
                        .WithOne("Token")
                        .HasForeignKey("osuRequestor.Models.TokenModel", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("osuRequestor.Models.UserModel", b =>
                {
                    b.Navigation("Token")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
