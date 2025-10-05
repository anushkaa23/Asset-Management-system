using System;
using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Business.DTOs
{
    public class AssignmentDTO
    {
        public int AssignmentId { get; set; }

        [Required(ErrorMessage = "Please select an asset")]
        public int AssetId { get; set; }

        [Required(ErrorMessage = "Please select an employee")]
        public int EmployeeId { get; set; }

        [Required]
        public DateTime AssignedDate { get; set; } = DateTime.Now;

        public DateTime? ReturnedDate { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        // For display purposes
        public string AssetName { get; set; }
        public string EmployeeName { get; set; }
        public bool IsActive { get; set; }
    }
}