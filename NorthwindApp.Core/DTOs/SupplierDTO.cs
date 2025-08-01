﻿namespace NorthwindApp.Core.DTOs
{
    public class SupplierDTO
    {
        public int SupplierId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? ContactName { get; set; }
        public string? Country { get; set; }
    }

    public class SupplierCreateDto
    {
        public string CompanyName { get; set; } = null!;
        public string? ContactName { get; set; }
        public string? ContactTitle { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
    }

    public class SupplierUpdateDto : SupplierCreateDto
    {
        public int SupplierId { get; set; }
    }
}
