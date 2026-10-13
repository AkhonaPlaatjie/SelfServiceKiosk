namespace SelfServiceKiosk.Api.Models;

/// <summary> A group that products belong to.</summary>
public class Category
{
    public int Id { get; set; }

    public string Name {get; set; } = string.Empty;

    public ICollection<Product> Products {get; set; } = new List<Product>();
}
