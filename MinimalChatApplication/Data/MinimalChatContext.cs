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
            modelBuilder.Entity<Channels>().ToTable("Channels");
            modelBuilder.Entity<ChannelMember>().ToTable("ChannelMember");


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

            modelBuilder.Entity<ChannelMember>()
              .HasKey(cm => new { cm.UserId, cm.ChannelId });
        }
        
        public DbSet<Message> Messages { get; set; }
        public DbSet<Logs> Log { get; set; }

        public DbSet<Channels> Channel { get; set; }

        public DbSet<ChannelMember> ChannelMembers { get; set; }


    }

}
