using BankingControlPanel.Core.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // Gets or sets the clients in the application.
        public DbSet<Client> Clients { get; set; }

        // Gets or sets the addresses in the application.
        public DbSet<Address> Addresses { get; set; }

        // Gets or sets the accounts in the application.
        public DbSet<Account> Accounts { get; set; }

        // Gets or sets the search parameters in the application.
        public DbSet<SearchParameter> SearchParameters { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the one-to-one relationship between Client and Address.
            // Each Client has one Address and each Address belongs to one Client.
            modelBuilder.Entity<Client>()
                .HasOne(c => c.Address)        // Each Client has one Address.
                .WithOne(a => a.Client)        // Each Address belongs to one Client.
                .HasForeignKey<Address>(a => a.ClientId); // The foreign key in Address is ClientId.

            // Configure the one-to-many relationship between Client and Account.
            // Each Client can have many Accounts and each Account belongs to one Client.
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Accounts)      // Each Client can have many Accounts.
                .WithOne(a => a.Client)        // Each Account belongs to one Client.
                .HasForeignKey(a => a.ClientId); // The foreign key in Account is ClientId.

            // Configure the one-to-many relationship between ApplicationUser (Admin) and SearchParameter.
            // Each ApplicationUser (Admin) can have many SearchParameters and each SearchParameter is associated with one Admin.
            modelBuilder.Entity<SearchParameter>()
                .HasOne(sp => sp.Admin)        // Each SearchParameter has one Admin.
                .WithMany()                    // Each Admin can have many SearchParameters.
                .HasForeignKey(sp => sp.AdminId); // The foreign key in SearchParameter is AdminId.

            // Seed roles
            var adminRoleId = Guid.NewGuid().ToString();
            var userRoleId = Guid.NewGuid().ToString();
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = userRoleId,
                    Name = "User",
                    NormalizedName = "USER"
                }
            );

            // Seed admin user
            var adminUserId = Guid.NewGuid().ToString();
            var hasher = new PasswordHasher<ApplicationUser>();
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = adminUserId,
                    UserName = "admin@admin.com",
                    NormalizedUserName = "ADMIN@ADMIN.COM",
                    Email = "admin@admin.com",
                    NormalizedEmail = "ADMIN@ADMIN.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Admin@123123"),
                    SecurityStamp = Guid.NewGuid().ToString()
                }
            );

            // Seed regular user
            var userUserId = Guid.NewGuid().ToString();
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = userUserId,
                    UserName = "user@user.com",
                    NormalizedUserName = "USER@USER.COM",
                    Email = "user@user.com",
                    NormalizedEmail = "USER@USER.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "User@123123"),
                    SecurityStamp = Guid.NewGuid().ToString()
                }
            );

            // Assign roles to users
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    UserId = adminUserId,
                    RoleId = adminRoleId
                },
                new IdentityUserRole<string>
                {
                    UserId = userUserId,
                    RoleId = userRoleId
                }
            );
        }
    }

}
