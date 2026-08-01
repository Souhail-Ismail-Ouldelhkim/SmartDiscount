using System.ComponentModel.DataAnnotations;

namespace SmartDiscount.Catalog.API.CatalogModel;

public class CatalogBrand
{
    public CatalogBrand(string brand)
    {
        Brand = brand;
    }

    public int Id { get; set; }

    [Required]
    public string Brand { get; set; }
}

