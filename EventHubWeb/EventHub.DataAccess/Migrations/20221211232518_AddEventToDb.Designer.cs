﻿// <auto-generated />
using System;
using EventHub.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EventHub.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20221211232518_AddEventToDb")]
    partial class AddEventToDb
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Models.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("EventDescription")
                        .IsRequired()
                        .HasColumnType("varchar(250)");

                    b.Property<string>("EventLocation")
                        .IsRequired()
                        .HasColumnType("varchar(250)");

                    b.Property<string>("EventName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<int?>("EventPrice")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("events");
                });
#pragma warning restore 612, 618
        }
    }
}
