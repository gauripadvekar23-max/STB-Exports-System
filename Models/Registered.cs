using SQLite;

namespace STB_Exports.Models;

public class RegisteredCustomer
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Unique]
    public string Email { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;
}
