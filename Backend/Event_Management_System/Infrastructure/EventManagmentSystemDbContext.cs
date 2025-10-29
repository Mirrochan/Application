using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class EventManagmentSystemDbContext: DbContext
    {
        public EventManagmentSystemDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<EventModel> Events { get; set; }
        public DbSet<TagModel> Tags { get; set; }
        public DbSet<AiConversation> AiConversations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventModel>()
                .HasOne(e => e.Organizer)
                .WithMany(u => u.OrganizedEvents)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventModel>()
                .HasMany(e => e.Participants)
                .WithMany(u => u.ParticipatingEvents)
                .UsingEntity(j => j.ToTable("EventParticipants"));
            
            modelBuilder.Entity<EventModel>()
                .HasMany(e => e.Tags)
                .WithMany(t => t.Events);
           
        }

    }
}
