﻿// <auto-generated />
using MFQ2022;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace TestApp.Migrations
{
    [DbContext(typeof(ProcessDbContext))]
    [Migration("20230514072353_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("MFQ2022.Process", b =>
                {
                    b.Property<int>("No")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ArrivingTime")
                        .HasColumnType("int");

                    b.Property<int>("CompleteTime")
                        .HasColumnType("int");

                    b.Property<int>("ExcutingTime")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<int>("RemainingTime")
                        .HasColumnType("int");

                    b.HasKey("No");

                    b.ToTable("Processes");
                });
#pragma warning restore 612, 618
        }
    }
}