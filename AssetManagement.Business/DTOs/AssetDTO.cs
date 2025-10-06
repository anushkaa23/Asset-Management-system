using System;
using System.ComponentModel.DataAnnotations;
using AssetManagement.Data.Entities;

namespace AssetManagement.Business.DTOs
{
    public class AssetDTO
    {
        public int AssetId { get; set; }

        [Required(ErrorMessage = "Asset Name is required")]
        [StringLength(100)]
        public string AssetName { get; set; }

        [Required(ErrorMessage = "Asset Type is required")]
        [StringLength(50)]
        public string AssetType { get; set; }

        [StringLength(100)]
        public string MakeModel { get; set; }

        [StringLength(100)]
        public string SerialNumber { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public DateTime? WarrantyExpiryDate { get; set; }

        [Required]
        public AssetCondition Condition { get; set; } = AssetCondition.Good;

        [Required]
        public AssetStatus Status { get; set; } = AssetStatus.Available;

        public bool IsSpare { get; set; } = false;

        [StringLength(1000)]
        public string Specifications { get; set; }

        public DateTime WarrantyExpiry { get; set; }
    }
}
