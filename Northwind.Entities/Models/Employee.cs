﻿namespace NorthwindApp.Entities.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Title { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
