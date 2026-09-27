using SQLite;

namespace STB_Exports.Models;

public class OrderNotification
{
    [PrimaryKey]
    [AutoIncrement]
    public int NotificationId { get; set; }

    // Customer who should receive this notification
    public int CustomerId { get; set; }

    // Related order
    public int OrderId { get; set; }

    public string OrderNumber { get; set; } = "";

    // Example:
    // "Your order STB202609080001 is Confirmed."
    public string Message { get; set; } = "";

    // Pending / Confirmed / Shipped / etc.
    public string OrderStatus { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // False = unread
    // True = read
    public bool IsRead { get; set; } = false;
}