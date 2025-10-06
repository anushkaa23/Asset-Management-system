using System;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Data.Models
{
    public class Asset
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Asset name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Make/Model is required")]
        public string MakeModel { get; set; }

        [DataType(DataType.Date)]
        public DateTime? PurchaseDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? WarrantyExpiryDate { get; set; }

        [Required(ErrorMessage = "Condition is required")]
        public string Condition { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; }

        public bool IsSpare { get; set; }

        public string Specifications { get; set; }

        // Optional foreign key to category (if used)
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        // Optional audit fields (if you plan to use them)
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
