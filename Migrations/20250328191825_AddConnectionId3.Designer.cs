﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TestSignalR;

#nullable disable

namespace TestSignalR.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250328191825_AddConnectionId3")]
    partial class AddConnectionId3
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.3");

            modelBuilder.Entity("TestSignalR.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Avatar")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.PrimitiveCollection<string>("Connections")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("RegistredAt")
                        .HasColumnType("TEXT");

                    b.Property<int?>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TestSignalR.Models.User", b =>
                {
                    b.HasOne("TestSignalR.Models.User", null)
                        .WithMany("Contacts")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("TestSignalR.Models.User", b =>
                {
                    b.Navigation("Contacts");
                });
#pragma warning restore 612, 618
        }
    }
}
