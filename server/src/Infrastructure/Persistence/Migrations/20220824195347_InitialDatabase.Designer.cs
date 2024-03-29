﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Prezentex.Infrastructure.Persistence.Repositories;

#nullable disable

namespace Prezentex.Application.Persistence.Migrations
{
    [DbContext(typeof(EntitiesDbContext))]
    [Migration("20220824195347_InitialDatabase")]
    partial class InitialDatabase
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseSerialColumns(modelBuilder);

            modelBuilder.Entity("Prezentex.Entities.Gift", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<string>("ProductUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("RecipientId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("RecipientId");

                    b.ToTable("Gifts");
                });

            modelBuilder.Entity("Prezentex.Entities.Recipient", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("BirthDay")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("NameDay")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Recipients");
                });

            modelBuilder.Entity("Prezentex.Entities.Gift", b =>
                {
                    b.HasOne("Prezentex.Entities.Recipient", null)
                        .WithMany("Gifts")
                        .HasForeignKey("RecipientId");
                });

            modelBuilder.Entity("Prezentex.Entities.Recipient", b =>
                {
                    b.Navigation("Gifts");
                });
#pragma warning restore 612, 618
        }
    }
}
