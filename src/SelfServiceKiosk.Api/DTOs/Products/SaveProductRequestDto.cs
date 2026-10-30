using System.ComponentModel.DataAnnotations;

namespace SelfServiceKiosk.Api.DTOs.Products;

public class SaveProductRequestDto
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name must be at most 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description must be at most 500 characters.")]
    public string? Description { get; set; }

    [Range(0.01, 99999999.99, ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Category is required.")]
    public int CategoryId { get; set; }

    [Url(ErrorMessage = "Image URL must be a valid web address.")]
    [StringLength(500, ErrorMessage = "Image URL must be at most 500 characters.")]
    public string? ImageUrl { get; set; }

    public bool IsAvailable { get; set; } = true;
}
