﻿// <auto-generated />
using System;
using BonyanTemplate.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BonyanTemplate.Infrastructure.Migrations
{
    [DbContext(typeof(TemplateBookManagementBonDbContext))]
    partial class TemplateBookManagementBonDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("Bonyan.IdentityManagement.Domain.Permissions.BonIdentityPermission", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Key");

                    b.ToTable("Permissions", (string)null);
                });

            modelBuilder.Entity("Bonyan.IdentityManagement.Domain.Roles.BonIdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT")
                        .HasColumnName("Id");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Roles", (string)null);
                });

            modelBuilder.Entity("Bonyan.IdentityManagement.Domain.Users.BonIdentityUserRoles", b =>
                {
                    b.Property<string>("RoleId")
                        .HasColumnType("TEXT")
                        .HasColumnName("RoleId");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT")
                        .HasColumnName("UserId");

                    b.HasKey("RoleId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRoles", (string)null);
                });

            modelBuilder.Entity("Bonyan.IdentityManagement.Domain.Users.BonIdentityUserToken", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT")
                        .HasColumnName("Id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Expiration")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT")
                        .HasColumnName("UserId");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserTokens", (string)null);
                });

            modelBuilder.Entity("Bonyan.TenantManagement.Domain.BonTenant", b =>
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

            modelBuilder.Entity("BonyanTemplate.Domain.Authors.Authors", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT")
                        .HasColumnName("Id");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("BonyanTemplate.Domain.Books.Book", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT")
                        .HasColumnName("Id");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("TEXT")
                        .HasColumnName("AuthorId");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("BonyanTemplate.Domain.Users.User", b =>
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

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Version")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("RolePermissions", b =>
                {
                    b.Property<string>("BonIdentityRoleId")
                        .HasColumnType("TEXT");

                    b.Property<string>("PermissionsKey")
                        .HasColumnType("TEXT");

                    b.HasKey("BonIdentityRoleId", "PermissionsKey");

                    b.HasIndex("PermissionsKey");

                    b.ToTable("RolePermissions");
                });

            modelBuilder.Entity("Bonyan.IdentityManagement.Domain.Users.BonIdentityUserRoles", b =>
                {
                    b.HasOne("Bonyan.IdentityManagement.Domain.Roles.BonIdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BonyanTemplate.Domain.Users.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Bonyan.IdentityManagement.Domain.Users.BonIdentityUserToken", b =>
                {
                    b.HasOne("BonyanTemplate.Domain.Users.User", null)
                        .WithMany("Tokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BonyanTemplate.Domain.Books.Book", b =>
                {
                    b.HasOne("BonyanTemplate.Domain.Authors.Authors", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("BonyanTemplate.Domain.Users.User", b =>
                {
                    b.OwnsOne("Bonyan.IdentityManagement.Domain.Users.ValueObjects.BonUserPassword", "Password", b1 =>
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

                    b.OwnsOne("Bonyan.UserManagement.Domain.Users.Enumerations.UserStatus", "Status", b1 =>
                        {
                            b1.Property<Guid>("UserId")
                                .HasColumnType("TEXT");

                            b1.Property<int>("Id")
                                .HasColumnType("INTEGER")
                                .HasColumnName("StatusId");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("TEXT")
                                .HasColumnName("StatusName");

                            b1.HasKey("UserId");

                            b1.ToTable("Users");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.OwnsOne("Bonyan.UserManagement.Domain.Users.ValueObjects.BonUserEmail", "Email", b1 =>
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

                    b.OwnsOne("Bonyan.UserManagement.Domain.Users.ValueObjects.BonUserPhoneNumber", "PhoneNumber", b1 =>
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

                    b.Navigation("Status")
                        .IsRequired();
                });

            modelBuilder.Entity("RolePermissions", b =>
                {
                    b.HasOne("Bonyan.IdentityManagement.Domain.Roles.BonIdentityRole", null)
                        .WithMany()
                        .HasForeignKey("BonIdentityRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bonyan.IdentityManagement.Domain.Permissions.BonIdentityPermission", null)
                        .WithMany()
                        .HasForeignKey("PermissionsKey")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BonyanTemplate.Domain.Users.User", b =>
                {
                    b.Navigation("Tokens");
                });
#pragma warning restore 612, 618
        }
    }
}
