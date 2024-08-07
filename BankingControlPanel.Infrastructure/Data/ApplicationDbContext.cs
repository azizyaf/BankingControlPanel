using BankingControlPanel.Core.Models.Entities;
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
        }
    }

}
