using WestmarchBook.Domain.Enums;

namespace WestmarchBook.Domain.ValueObjects;

public class Address
{
    public string Street { get; set; } = string.Empty;     
    public string Number { get; set; } = string.Empty;    
    public string Neighborhood { get; set; } = string.Empty; 
    public string City { get; set; } = string.Empty;
    public StateUf StateUf { get; set; } = StateUf.BA;     
    public string ZipCode { get; set; } = string.Empty;    
}
