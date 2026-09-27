using SQLite;

namespace STB_Exports.Models;

public class SavedAddress
{
    [PrimaryKey]
    [AutoIncrement]
    public int AddressId { get; set; }

    public int CustomerId { get; set; }

    public string FullName { get; set; } = "";

    public string PhoneNumber { get; set; } = "";

    public string Address { get; set; } = "";

    public string City { get; set; } = "";

    public string State { get; set; } = "";

    public string Pincode { get; set; } = "";

    public string AddressType { get; set; } = "Home";


    // =====================================================
    // DISPLAY PROPERTIES
    // =====================================================

    [Ignore]
    public string FullAddress
    {
        get
        {
            return Address;
        }
    }


    [Ignore]
    public string CityStatePincode
    {
        get
        {
            return $"{City}, {State} - {Pincode}";
        }
    }


    [Ignore]
    public string DisplayName
    {
        get
        {
            return FullName;
        }
    }


    [Ignore]
    public string DisplayPhone
    {
        get
        {
            return PhoneNumber;
        }
    }
}