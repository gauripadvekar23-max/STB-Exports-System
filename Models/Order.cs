using SQLite;

namespace STB_Exports.Models;

public class Order
{
    [PrimaryKey]
    [AutoIncrement]
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    // Example: STB202608280001
    public string OrderNumber { get; set; } = "";

    public DateTime OrderDate { get; set; } = DateTime.Now;

    // Expected delivery date
    public DateTime ExpectedDeliveryDate { get; set; }

    // Pending / Confirmed / Shipped / OutForDelivery / Delivered / Cancelled
    public string OrderStatus { get; set; } = "Pending";

    // COD / UPI / Card
    public string PaymentMethod { get; set; } = "";

    // Pending / Paid / Failed
    public string PaymentStatus { get; set; } = "Pending";

    // Delivery information
    public string FullName { get; set; } = "";

    public string PhoneNumber { get; set; } = "";

    public string Address { get; set; } = "";

    public string City { get; set; } = "";

    public string State { get; set; } = "";

    public string Pincode { get; set; } = "";

    // Final order amount
    public decimal TotalAmount { get; set; }

    [Ignore]
    public string ProductNames { get; set; } = "";

    [Ignore]
    public string ProductDetails { get; set; } = "";
}