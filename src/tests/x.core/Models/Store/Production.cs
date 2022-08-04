using Ws.Core.Extensions.Data;

namespace x.core.Models.Store;

public class Brand: Entity<int> {
    public string? Name { get; set; }
}
public class Category : Entity<int>
{
    public string? Name { get; set; }
}
public class Product : Entity<int>
{
    public string? Name { get; set; }
    public virtual Brand? Brand { get; set; }
    public virtual Category? Category { get; set; }
    public short Model_Year { get; set; }
    public decimal List_Price { get; set; }    
}
