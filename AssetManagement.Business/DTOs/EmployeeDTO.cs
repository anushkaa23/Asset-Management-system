using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Business.DTOs
{
    public class EmployeeDTO
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Department is required")]
        [StringLength(50)]
        public string Department { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [Phone(ErrorMessage = "Invalid phone format")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Designation is required")]
        [StringLength(50)]
        public string Designation { get; set; }

        public bool IsActive { get; set; } = true;
    }
}