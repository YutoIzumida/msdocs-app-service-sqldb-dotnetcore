using System;
using System.ComponentModel.DataAnnotations;

namespace DotNetCoreSqlDb.Models
{
    public class Price
    {
        public int ID { get; set; }

        [Required]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}
