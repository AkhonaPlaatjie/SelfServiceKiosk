using System.ComponentModel.DataAnnotations;

namespace SelfServiceKiosk.Api.DTOs.Categories;

public class SaveCategoryRequestDto
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, ErrorMessage = "Name must be at most 50 characters.")]
    public string Name { get; set; } = string.Empty;
}