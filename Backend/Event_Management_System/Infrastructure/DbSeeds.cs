using Domain.Models;
using Infrastructure.Jwt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(EventManagmentSystemDbContext context)
        {
            if (await context.Users.AnyAsync()) return;

            var passwordHasher = new PasswordHasher();
            var Tags = new List<TagModel>
            {
                new TagModel { Id = Guid.NewGuid(), Name = "Technology", Color = "#3B82F6" },
                new TagModel { Id = Guid.NewGuid(), Name = "Programming", Color = "#8B5CF6" },
                new TagModel { Id = Guid.NewGuid(), Name = "AI & Machine Learning", Color = "#10B981" },

                new TagModel { Id = Guid.NewGuid(), Name = "Business", Color = "#059669" },
                new TagModel { Id = Guid.NewGuid(), Name = "Startups", Color = "#DC2626" },
                new TagModel { Id = Guid.NewGuid(), Name = "Networking", Color = "#0EA5E9" },
                new TagModel { Id = Guid.NewGuid(), Name = "Marketing", Color = "#F97316" },
                new TagModel { Id = Guid.NewGuid(), Name = "Finance", Color = "#14B8A6" },

                new TagModel { Id = Guid.NewGuid(), Name = "Art", Color = "#8B5CF6" },
                new TagModel { Id = Guid.NewGuid(), Name = "Music", Color = "#EF4444" },
                new TagModel { Id = Guid.NewGuid(), Name = "Photography", Color = "#F59E0B" },
                new TagModel { Id = Guid.NewGuid(), Name = "Writing", Color = "#10B981" },
                new TagModel { Id = Guid.NewGuid(), Name = "Dance", Color = "#DC2626" },

                new TagModel { Id = Guid.NewGuid(), Name = "Education", Color = "#0EA5E9" },
                new TagModel { Id = Guid.NewGuid(), Name = "Workshop", Color = "#84CC16" },
                new TagModel { Id = Guid.NewGuid(), Name = "Conference", Color = "#8B5CF6" },
                new TagModel { Id = Guid.NewGuid(), Name = "Training", Color = "#10B981" },

                new TagModel { Id = Guid.NewGuid(), Name = "Charity", Color = "#EF4444" },
                new TagModel { Id = Guid.NewGuid(), Name = "Volunteering", Color = "#10B981" },

                new TagModel { Id = Guid.NewGuid(), Name = "Sports", Color = "#DC2626" },
                new TagModel { Id = Guid.NewGuid(), Name = "Cooking", Color = "#DC2626" },
                new TagModel { Id = Guid.NewGuid(), Name = "Gaming", Color = "#8B5CF6" },
                new TagModel { Id = Guid.NewGuid(), Name = "Movies", Color = "#6366F1" },

                new TagModel { Id = Guid.NewGuid(), Name = "Health", Color = "#10B981" },
                new TagModel { Id = Guid.NewGuid(), Name = "Travel", Color = "#0EA5E9" },
                new TagModel { Id = Guid.NewGuid(), Name = "Language Exchange", Color = "#EF4444" },
                new TagModel { Id = Guid.NewGuid(), Name = "Environment", Color = "#84CC16" }
            };
            await context.Tags.AddRangeAsync(Tags);
  

            var users = new List<UserModel>
            {
                new() { Id = Guid.NewGuid(), Email = "john.doe@example.com", FirstName = "John", LastName = "Doe", PasswordHash = passwordHasher.Generate("Password123!") },
                new() { Id = Guid.NewGuid(), Email = "jane.smith@example.com", FirstName = "Jane", LastName = "Smith", PasswordHash = passwordHasher.Generate("Password123!") },
                new() { Id = Guid.NewGuid(), Email = "mark.taylor@example.com", FirstName = "Mark", LastName = "Taylor", PasswordHash = passwordHasher.Generate("Password123!") },
                new() { Id = Guid.NewGuid(), Email = "emily.johnson@example.com", FirstName = "Emily", LastName = "Johnson", PasswordHash = passwordHasher.Generate("Password123!") },
                new() { Id = Guid.NewGuid(), Email = "sarah.lee@example.com", FirstName = "Sarah", LastName = "Lee", PasswordHash = passwordHasher.Generate("Password123!") },
                new() { Id = Guid.NewGuid(), Email = "michael.white@example.com", FirstName = "Michael", LastName = "White", PasswordHash = passwordHasher.Generate("Password123!") },
                new() { Id = Guid.NewGuid(), Email = "linda.green@example.com", FirstName = "Linda", LastName = "Green", PasswordHash = passwordHasher.Generate("Password123!") },
                new() { Id = Guid.NewGuid(), Email = "peter.harris@example.com", FirstName = "Peter", LastName = "Harris", PasswordHash = passwordHasher.Generate("Password123!") },
                new() { Id = Guid.NewGuid(), Email = "anna.martin@example.com", FirstName = "Anna", LastName = "Martin", PasswordHash = passwordHasher.Generate("Password123!") },
                new() { Id = Guid.NewGuid(), Email = "robert.brown@example.com", FirstName = "Robert", LastName = "Brown", PasswordHash = passwordHasher.Generate("Password123!") }
            };

            await context.Users.AddRangeAsync(users);
  

            var events = new List<EventModel>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Tech Innovators Summit 2025",
                    Description = "A three-day international conference bringing together innovators, developers, and entrepreneurs from around the globe. Attendees will explore the latest trends in artificial intelligence, cybersecurity, and software engineering. The summit includes keynote speeches by industry leaders, panel discussions, startup exhibitions, and live coding sessions designed to inspire and connect professionals in the tech ecosystem.",
                    Date = DateTime.UtcNow.AddDays(15),
                    Location = "Innovation Center, San Francisco",
                    Capacity = 12,
                    IsPublic = true,
                    OrganizerId = users[0].Id,
                    Organizer = users[0],
                    Participants = new List<UserModel>
                    {
                        users[1], users[2], users[3], users[4], users[5]
                    },
                    Tags = new List<TagModel>
                    {
                        Tags[2], Tags[15], Tags[16], Tags[10], Tags[11]
                    }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Green Future Forum",
                    Description = "A public event focused on sustainable technologies and environmental protection. It includes discussions on renewable energy, eco-friendly production, and climate change solutions. Ideal for students, researchers, and environmental activists.",
                    Date = DateTime.UtcNow.AddDays(30),
                    Location = "EcoHub, Seattle",
                    Capacity = 32,
                    IsPublic = true,
                    OrganizerId = users[3].Id,
                    Organizer = users[3],
                    Participants = new List<UserModel>
                    {
                        users[0], users[6], users[8]
                    },
                    Tags = new List<TagModel>
                    {
                        Tags[0], Tags[9], Tags[4]
                    }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Private Leadership Workshop",
                    Description = "An exclusive internal workshop aimed at developing leadership and management skills among company executives. The program includes interactive sessions, team challenges, and psychological training to improve communication and decision-making.",
                    Date = DateTime.UtcNow.AddDays(10),
                    Location = "Lakeview Retreat Center, Denver",
                    Capacity = 3,
                    IsPublic = true,
                    OrganizerId = users[5].Id,
                    Organizer = users[5],
                    Participants = new List<UserModel>
                    {
                        users[7], users[8], users[9]
                    },
                    Tags = new List<TagModel>
                    {
                        Tags[2], Tags[4], Tags[21], Tags[0]
                    }
                }
            };

            await context.Events.AddRangeAsync(events);
            await context.SaveChangesAsync();

        }


    }
}