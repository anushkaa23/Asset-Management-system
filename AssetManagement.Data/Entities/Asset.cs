using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Data.Entities
{
    public enum AssetCondition
    {
        New,
        Good,
        NeedsRepair,
        Damaged
    }

    public enum AssetStatus
    {
        Available,
        Assigned,
        UnderRepair,
        Retired
    }

    public class Asset
    {
        [Key]
        public int AssetId { get; set; }

        [Required(ErrorMessage = "Asset Name is required")]
        [MaxLength(100)]
        public string AssetName { get; set; }

        [Required(ErrorMessage = "Asset Type is required")]
        [MaxLength(50)]
        public string AssetType { get; set; }

        [MaxLength(100)]
        public string MakeModel { get; set; }

        [MaxLength(100)]
        public string SerialNumber { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public DateTime? WarrantyExpiryDate { get; set; }

        [Required]
        public AssetCondition Condition { get; set; } = AssetCondition.Good;

        [Required]
        public AssetStatus Status { get; set; } = AssetStatus.Available;

        public bool IsSpare { get; set; } = false;

        [StringLength(1000)]
        public string? Specifications { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }

        // Navigation property
        public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}
