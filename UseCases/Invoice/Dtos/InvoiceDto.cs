namespace PaginationResultWebApi.UseCases.Invoice.Dtos;

public class InvoiceDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string StreetAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public List<InvoiceItem> Items { get; set; } = [];
}

public class InvoiceItem
{
    public string Description { get; set; } = string.Empty;
    public string Quantity { get; set; } = string.Empty;
    public string UnitPrice { get; set; } = string.Empty;
    public string TotalAmount { get; set; } = string.Empty;
}
