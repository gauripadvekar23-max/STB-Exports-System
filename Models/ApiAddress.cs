namespace STB_Exports.Models;

public class ApiAddress
{
    public string AddressId { get; set; } = string.Empty;

    public string CustomerId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string AddressLine { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string Pincode { get; set; } = string.Empty;

    public bool IsDefault { get; set; }
}
public class ApiAddressDisplayItem
{
    public string AddressId { get; set; } = string.Empty;

    public string CustomerId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string Pincode { get; set; } = string.Empty;

    public string AddressType { get; set; } = "Home";

    public bool IsDefault { get; set; }

    public string CityStatePincode =>
        $"{City}, {State} - {Pincode}";
}