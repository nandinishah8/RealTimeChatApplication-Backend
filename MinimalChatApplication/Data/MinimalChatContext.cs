using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MinimalChatApplication.Models;
using System.Collections.Generic;

namespace MinimalChatApplication.Data
{
    
    public class MinimalChatContext : IdentityDbContext
    {
        public MinimalChatContext(DbContextOptions<MinimalChatContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Message>().ToTable("Message");
            modelBuilder.Entity<Logs>().ToTable("Log");


           modelBuilder.Entity<Message>()
               .HasOne(m => m.Receiver)
               .WithMany()
               .HasForeignKey(m => m.ReceiverId)
               .OnDelete(DeleteBehavior.Restrict);
            //configure sender
            modelBuilder.Entity<Message>()
              .HasOne(m => m.Sender)
              .WithMany()
              .HasForeignKey(m => m.SenderId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });
            });
        }
        
        public DbSet<Message> Messages { get; set; }
        public DbSet<Logs> Log { get; set; }


    }

}
