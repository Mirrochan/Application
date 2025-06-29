﻿namespace DeskBookingAPI.Models
{
    public class CoworkingModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public ICollection<Workspace> Workspaces { get; set; } = new List<Workspace>();
    }
}
