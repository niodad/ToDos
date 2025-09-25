using System.ComponentModel.DataAnnotations;

namespace ToDos.Domain.Entities
{
    public class ToDo
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string? Name { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string Email { get; set; } = default!;
        
        [Required(ErrorMessage = "Date is required")]
        public DateTimeOffset Date { get; set; } = default!;
        
        public bool Done { get; set; }
    }
}
