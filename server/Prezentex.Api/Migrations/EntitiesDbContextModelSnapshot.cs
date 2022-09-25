﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Prezentex.Api.Repositories;

#nullable disable

namespace Prezentex.Api.Migrations
{
    [DbContext(typeof(EntitiesDbContext))]
    partial class EntitiesDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseSerialColumns(modelBuilder);

            modelBuilder.Entity("Prezentex.Api.Entities.Gift", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<string>("ProductUrl")
                        .HasColumnType("text");

                    b.Property<Guid?>("RecipientId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("RecipientId");

                    b.HasIndex("UserId");

                    b.ToTable("Gifts");
                });

            modelBuilder.Entity("Prezentex.Api.Entities.Recipient", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("BirthDay")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("NameDay")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Note")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Recipients");
                });

            modelBuilder.Entity("Prezentex.Api.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Prezentex.Api.Entities.Gift", b =>
                {
                    b.HasOne("Prezentex.Api.Entities.Recipient", null)
                        .WithMany("Gifts")
                        .HasForeignKey("RecipientId");

                    b.HasOne("Prezentex.Api.Entities.User", null)
                        .WithMany("Gifts")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Prezentex.Api.Entities.Recipient", b =>
                {
                    b.HasOne("Prezentex.Api.Entities.User", null)
                        .WithMany("Recipients")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Prezentex.Api.Entities.Recipient", b =>
                {
                    b.Navigation("Gifts");
                });

            modelBuilder.Entity("Prezentex.Api.Entities.User", b =>
                {
                    b.Navigation("Gifts");

                    b.Navigation("Recipients");
                });
#pragma warning restore 612, 618
        }
    }
}
