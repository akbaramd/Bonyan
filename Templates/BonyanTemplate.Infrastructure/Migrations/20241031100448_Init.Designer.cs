﻿// <auto-generated />
using System;
using BonyanTemplate.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BonyanTemplate.Infrastructure.Migrations
{
    [DbContext(typeof(BonyanTemplateBookManagementDbContext))]
    [Migration("20241031100448_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("Bonyan.TenantManagement.Domain.Tenant", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT")
                        .HasColumnName("Id");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("CreatedDate");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("DeletedDate");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(false)
                        .HasColumnName("IsDeleted");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("ModifiedDate");

                    b.HasKey("Id");

                    b.HasIndex("Key")
                        .IsUnique();

                    b.ToTable("Tenants");
                });

            modelBuilder.Entity("Bonyan.UserManagement.Domain.Enumerations.UserStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("UserStatus");
                });

            modelBuilder.Entity("BonyanTemplate.Domain.Entities.Authors", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT")
                        .HasColumnName("Id");

                    b.Property<Guid?>("TenantId")
                        .HasColumnType("TEXT")
                        .HasColumnName("TenantId");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("BonyanTemplate.Domain.Entities.Books", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT")
                        .HasColumnName("Id");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("TenantId")
                        .HasColumnType("TEXT")
                        .HasColumnName("TenantId");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("BonyanTemplate.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT")
                        .HasColumnName("Id");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("CreatedDate");

                    b.Property<DateTime?>("DeletedDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("DeletedDate");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(false)
                        .HasColumnName("IsDeleted");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("TEXT")
                        .HasColumnName("ModifiedDate");

                    b.Property<int>("StatusId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("StatusId");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BonyanTemplate.Domain.Entities.User", b =>
                {
                    b.HasOne("Bonyan.UserManagement.Domain.Enumerations.UserStatus", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("Bonyan.UserManagement.Domain.ValueObjects.Email", "Email", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Address")
                                .IsRequired()
                                .HasColumnType("TEXT")
                                .HasColumnName("EmailAddress");

                            b1.Property<bool>("IsVerified")
                                .HasColumnType("INTEGER")
                                .HasColumnName("EmailIsVerified");

                            b1.HasKey("UserId");

                            b1.ToTable("Users");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.OwnsOne("Bonyan.UserManagement.Domain.ValueObjects.Password", "Password", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("TEXT");

                            b1.Property<string>("HashedPassword")
                                .IsRequired()
                                .HasColumnType("TEXT")
                                .HasColumnName("PasswordHash");

                            b1.Property<byte[]>("Salt")
                                .IsRequired()
                                .HasColumnType("BLOB")
                                .HasColumnName("PasswordSalt");

                            b1.HasKey("UserId");

                            b1.ToTable("Users");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.OwnsOne("Bonyan.UserManagement.Domain.ValueObjects.PhoneNumber", "PhoneNumber", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("TEXT");

                            b1.Property<bool>("IsVerified")
                                .HasColumnType("INTEGER")
                                .HasColumnName("PhoneNumberIsVerified");

                            b1.Property<string>("Number")
                                .IsRequired()
                                .HasColumnType("TEXT")
                                .HasColumnName("PhoneNumber");

                            b1.HasKey("UserId");

                            b1.ToTable("Users");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.Navigation("Email");

                    b.Navigation("Password")
                        .IsRequired();

                    b.Navigation("PhoneNumber");

                    b.Navigation("Status");
                });
#pragma warning restore 612, 618
        }
    }
}
