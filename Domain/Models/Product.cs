using Newtonsoft.Json;

namespace Domain.Models;

public class Product
{
    // Mengabaikan properti Id saat serialisasi kecuali nilai default (0 pada int) diganti
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)] 
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
