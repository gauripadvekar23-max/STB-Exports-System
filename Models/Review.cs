using SQLite;

namespace STB_Exports.Models;

public class Review
{
    [PrimaryKey]
    [AutoIncrement]
    public int ReviewId { get; set; }

    public int CustomerId { get; set; }

    public int ProductId { get; set; }

    public int Rating { get; set; }

    public string ReviewText { get; set; } = "";

    public string ReviewDate { get; set; } = "";

    // Customer name is used only for displaying the review.
    [Ignore]
    public string CustomerName { get; set; } = "";

    // Product name is used only for displaying the review.
    [Ignore]
    public string ProductName { get; set; } = "";

    // Product image is used only for displaying the review.
    [Ignore]
    public string ProductImage { get; set; } = "";
}