namespace SelfServiceKiosk.Api.Models;

/// <summary>A group that products belong to.</summary>
public class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>False means deactivated (a soft delete): the row stays, but the category is hidden from the kiosk.</summary>
    public bool IsActive { get; set; } = true;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
