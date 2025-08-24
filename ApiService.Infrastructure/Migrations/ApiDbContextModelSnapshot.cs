using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ApiService.Infrastructure.Migrations
{
    [DbContext(typeof(ApiDbContext))]
    partial class ApiDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("ApiService.Domain.User", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<string>("Email")
                    .IsRequired()
                    .HasColumnType("nvarchar(450)");

                b.Property<string>("EmailVerificationToken")
                    .HasColumnType("nvarchar(max)");

                b.Property<bool>("IsEmailVerified")
                    .HasColumnType("bit");

                b.Property<string>("PasswordHash")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("PasswordResetToken")
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime?>("PasswordResetTokenExpiry")
                    .HasColumnType("datetime2");

                b.Property<string>("RefreshToken")
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime?>("RefreshTokenExpiry")
                    .HasColumnType("datetime2");

                b.HasKey("Id");

                b.HasIndex("Email")
                    .IsUnique();

                b.ToTable("Users");
            });
        }
    }
}
