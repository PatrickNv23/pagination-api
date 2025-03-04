namespace PaginationResultWebApi.Entities;

public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; }
    public string? Photo { get; set; }
    public string? Provider { get; set; }
}