namespace SelfServiceKiosk.Api.Models;

/// <summary>One movement of money on an account. The allowed types are not defined yet.</summary>
public class Transaction
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public Account Account { get; set; } = null!;

    public string Type { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string? Reference { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
