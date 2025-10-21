using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagement.Data.Entities
{
    public class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }

        [Required]
        public int AssetId { get; set; }

        public int? EmployeeId { get; set; } // Nullable now

        [Required]
        public DateTime AssignmentDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public DateTime? ModifiedDate { get; set; } // needed for repository updates

        public bool IsReturned { get; set; } = false;

        [MaxLength(500)]
        public string Notes { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("AssetId")]
        public virtual Asset Asset { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
    }
}
