namespace SelfServiceKiosk.Api.Models;

///<summary>A user's balance. Each user has exactly one account.</summary>
public class Account
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public decimal Balance { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
