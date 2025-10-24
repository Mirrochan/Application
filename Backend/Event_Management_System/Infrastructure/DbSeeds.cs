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

            var users = new[]
            {
                new UserModel
                {
                    Id = Guid.NewGuid(),
                    Email = "john@example.com",
                    FirstName = "John",
                    LastName = "Doe",
                    PasswordHash = passwordHasher.Generate("Password123!")
                },
                new UserModel
                {
                    Id = Guid.NewGuid(),
                    Email = "jane@example.com",
                    FirstName = "Jane",
                    LastName = "Smith",
                    PasswordHash = passwordHasher.Generate("Password123!")
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

           
            var events = new[]
            {
                new EventModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Tech Conference 2024",
                    Description = "Annual technology conference featuring the latest innovations",
                    Date = DateTime.UtcNow.AddDays(7),
                    Location = "Convention Center, New York",
                    Capacity = 100,
                    IsPublic = true,
                    OrganizerId = users[0].Id
                },
                new EventModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Startup Networking Event",
                    Description = "Connect with entrepreneurs and investors",
                    Date = DateTime.UtcNow.AddDays(14),
                    Location = "Innovation Hub, San Francisco",
                    Capacity = 50,
                    IsPublic = true,
                    OrganizerId = users[1].Id
                },
                new EventModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Private Team Building",
                    Description = "Company internal team building activity",
                    Date = DateTime.UtcNow.AddDays(21),
                    Location = "Adventure Park, Boston",
                    Capacity = 25,
                    IsPublic = false,
                    OrganizerId = users[0].Id
                }
            };

            await context.Events.AddRangeAsync(events);
            await context.SaveChangesAsync();
        }
    }
}